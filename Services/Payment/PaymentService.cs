using ECommerceAPI.Data;
using ECommerceAPI.DTOs.Payment;
using ECommerceAPI.Models;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ECommerceAPI.Services.Payments;

public class PaymentService : IPaymentService
{
    // ====================================================
    private readonly AppDbContext _db;
    private readonly IOptions<MercadoPagoSettings> _mp;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        AppDbContext db,
        IOptions<MercadoPagoSettings> mp,
        ILogger<PaymentService> logger)
    {
        _db = db;
        _mp = mp;
        _logger = logger;
    }
    // ====================================================

    public async Task<CreatePreferenceResponseDto> CreatePreferenceAsync(int userId)
    {
        var cartItems = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (cartItems.Count == 0)
            throw new InvalidOperationException("El carrito está vacío.");

        var totalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity);
        var reservationExpiresAt = DateTime.UtcNow.AddMinutes(15);  // La orden dura 15 minutos, si no paga, expira.

        await using var tx = await _db.Database.BeginTransactionAsync();

        // Verifica si hay stock.
        try
        {
            foreach (var item in cartItems)
            {
                var affectedRows = await _db.Database.ExecuteSqlInterpolatedAsync($@"
                    UPDATE ""Products""
                    SET ""ReservedStock"" = ""ReservedStock"" + {item.Quantity}
                    WHERE ""Id"" = {item.ProductId}
                      AND (""Stock"" - ""ReservedStock"") >= {item.Quantity};
                ");

                if (affectedRows == 0)
                {
                    await tx.RollbackAsync();
                    throw new InvalidOperationException(
                        $"No hay stock disponible suficiente para el producto '{item.Product.Name}'."
                    );
                }
            }

            // Si llega hasta acá es porque hay stock -> comienza la creación de la orden.
            var order = new Order
            {
                UserId = userId,
                TotalAmount = totalAmount,
                Status = OrderStatuses.AwaitingPayment,
                ReservationExpiresAt = reservationExpiresAt
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            var orderItems = cartItems.Select(i => new OrderItem
            {
                OrderId = order.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                PriceAtPurchase = i.Product.Price
            }).ToList();

            _db.OrderItems.AddRange(orderItems);
            await _db.SaveChangesAsync();

            MercadoPagoConfig.AccessToken = _mp.Value.AccessToken;

            var frontendBaseUrl = _mp.Value.FrontendBaseUrl;

            var preferenceRequest = new PreferenceRequest
            {
                Items = cartItems.Select(i => new PreferenceItemRequest
                {
                    Title = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.Product.Price,
                    CurrencyId = "UYU"
                }).ToList(),

                ExternalReference = order.Id.ToString(),

                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = $"{frontendBaseUrl}/checkout?status=success&orderId={order.Id}",
                    Failure = $"{frontendBaseUrl}/checkout?status=failure&orderId={order.Id}",
                    Pending = $"{frontendBaseUrl}/checkout?status=pending&orderId={order.Id}",
                },

                AutoReturn = "approved",
                NotificationUrl = $"{_mp.Value.WebhookBaseUrl}/api/payments/webhook"
            };

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(preferenceRequest);

            order.MercadoPagoPreferenceId = preference.Id;
            await _db.SaveChangesAsync();

            await tx.CommitAsync();

            _logger.LogInformation(
                "Reserva creada correctamente. orderId={OrderId}, userId={UserId}, expiresAt={ExpiresAt}",
                order.Id,
                userId,
                reservationExpiresAt
            );

            return new CreatePreferenceResponseDto
            {
                OrderId = order.Id,
                InitPoint = preference.InitPoint,
                SandboxInitPoint = preference.SandboxInitPoint
            };
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            _logger.LogError(ex, "Error al crear preferencia con reserva de stock. userId={UserId}", userId);
            throw new InvalidOperationException("No se pudo iniciar el checkout.");
        }
    }

    public async Task ProcessWebhookAsync(string? type, string? dataId, string? dataIdAlt)
    {
        var paymentIdStr = dataId ?? dataIdAlt;

        if (type != "payment" || string.IsNullOrWhiteSpace(paymentIdStr))
            return;

        if (!long.TryParse(paymentIdStr, out var parsedPaymentId))
        {
            _logger.LogWarning("Webhook ignorado: payment id inválido. dataId={DataId}", paymentIdStr);
            return;
        }

        MercadoPagoConfig.AccessToken = _mp.Value.AccessToken;

        var paymentClient = new PaymentClient();
        Payment payment;

        try
        {
            payment = await paymentClient.GetAsync(parsedPaymentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar el pago en Mercado Pago. paymentId={PaymentId}", parsedPaymentId);
            return;
        }

        if (string.IsNullOrWhiteSpace(payment.ExternalReference))
        {
            _logger.LogWarning("Pago sin ExternalReference. paymentId={PaymentId}", payment.Id);
            return;
        }

        if (!int.TryParse(payment.ExternalReference, out var orderId))
        {
            _logger.LogWarning(
                "ExternalReference inválida. paymentId={PaymentId}, externalReference={ExternalReference}",
                payment.Id,
                payment.ExternalReference
            );
            return;
        }

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                await tx.RollbackAsync();
                _logger.LogWarning("Orden no encontrada para webhook. orderId={OrderId}, paymentId={PaymentId}", orderId, payment.Id);
                return;
            }

            if (order.MercadoPagoPaymentId == payment.Id)
            {
                await tx.CommitAsync();
                _logger.LogInformation("Webhook repetido ignorado. orderId={OrderId}, paymentId={PaymentId}", order.Id, payment.Id);
                return;
            }

            var paymentAlreadyUsed = await _db.Orders
                .AnyAsync(o => o.Id != order.Id && o.MercadoPagoPaymentId == payment.Id);

            if (paymentAlreadyUsed)
            {
                await tx.CommitAsync();
                _logger.LogWarning("paymentId ya usado en otra orden. orderId={OrderId}, paymentId={PaymentId}", order.Id, payment.Id);
                return;
            }

            if (order.Status == OrderStatuses.Paid ||
                order.Status == OrderStatuses.Preparing ||
                order.Status == OrderStatuses.Shipped ||
                order.Status == OrderStatuses.Delivered ||
                order.Status == OrderStatuses.Cancelled ||
                order.Status == OrderStatuses.Expired)
            {
                await tx.CommitAsync();
                _logger.LogInformation("Webhook ignorado por estado final. orderId={OrderId}, status={Status}", order.Id, order.Status);
                return;
            }

            var paymentStatus = payment.Status;

            if (paymentStatus == "approved")
            {
                if (payment.TransactionAmount != order.TotalAmount)
                {
                    order.Status = OrderStatuses.Cancelled;
                    order.MercadoPagoPaymentId = payment.Id;
                    await _db.SaveChangesAsync();
                    await tx.CommitAsync();

                    _logger.LogWarning(
                        "Monto inválido. orderId={OrderId}, paymentId={PaymentId}, expected={Expected}, paid={Paid}",
                        order.Id,
                        payment.Id,
                        order.TotalAmount,
                        payment.TransactionAmount
                    );
                    return;
                }

                if (order.ReservationExpiresAt.HasValue && order.ReservationExpiresAt.Value < DateTime.UtcNow)
                {
                    order.Status = OrderStatuses.Expired;
                    order.MercadoPagoPaymentId = payment.Id;
                    await _db.SaveChangesAsync();
                    await tx.CommitAsync();

                    _logger.LogWarning(
                        "Pago aprobado pero la reserva ya había expirado. orderId={OrderId}, paymentId={PaymentId}",
                        order.Id,
                        payment.Id
                    );
                    return;
                }

                foreach (var item in order.Items)
                {
                    var affectedRows = await _db.Database.ExecuteSqlInterpolatedAsync($@"
                        UPDATE ""Products""
                        SET ""ReservedStock"" = ""ReservedStock"" - {item.Quantity},
                            ""TotalSold"" = ""TotalSold"" + {item.Quantity}
                        WHERE ""Id"" = {item.ProductId}
                          AND ""ReservedStock"" >= {item.Quantity};
                    ");

                    if (affectedRows == 0)
                    {
                        await tx.RollbackAsync();
                        _logger.LogError(
                            "No se pudo confirmar la reserva como venta. orderId={OrderId}, productId={ProductId}",
                            order.Id,
                            item.ProductId
                        );
                        return;
                    }
                }

                order.Status = OrderStatuses.Paid;
                order.MercadoPagoPaymentId = payment.Id;
                order.ReservationExpiresAt = null;

                var cartItems = await _db.CartItems
                    .Where(c => c.UserId == order.UserId)
                    .ToListAsync();

                _db.CartItems.RemoveRange(cartItems);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                _logger.LogInformation(
                    "Pago confirmado y reserva convertida en venta. orderId={OrderId}, paymentId={PaymentId}",
                    order.Id,
                    payment.Id
                );

                return;
            }

            if (paymentStatus == "rejected" || paymentStatus == "cancelled")
            {
                foreach (var item in order.Items)
                {
                    await _db.Database.ExecuteSqlInterpolatedAsync($@"
                        UPDATE ""Products""
                        SET ""ReservedStock"" = ""ReservedStock"" - {item.Quantity}
                        WHERE ""Id"" = {item.ProductId}
                          AND ""ReservedStock"" >= {item.Quantity};
                    ");
                }

                order.Status = OrderStatuses.Cancelled;
                order.MercadoPagoPaymentId = payment.Id;
                order.ReservationExpiresAt = null;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                _logger.LogInformation(
                    "Pago cancelado/rechazado y reserva liberada. orderId={OrderId}, paymentId={PaymentId}",
                    order.Id,
                    payment.Id
                );

                return;
            }

            await tx.CommitAsync();
            _logger.LogInformation(
                "Webhook recibido con estado intermedio. orderId={OrderId}, paymentId={PaymentId}, paymentStatus={PaymentStatus}",
                order.Id,
                payment.Id,
                paymentStatus
            );
        }
        catch (DbUpdateException ex)
        {
            await tx.RollbackAsync();
            _logger.LogError(ex, "DbUpdateException al procesar webhook. paymentId={PaymentId}", parsedPaymentId);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            _logger.LogError(ex, "Error inesperado al procesar webhook. paymentId={PaymentId}", parsedPaymentId);
            throw;
        }
    }

    public async Task<OrderStatusResponseDto> GetOrderStatusAsync(int orderId, int userId)
    {
        var order = await _db.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null)
            throw new KeyNotFoundException("Orden no encontrada.");

        return new OrderStatusResponseDto
        {
            OrderId = order.Id,
            Status = order.Status,
            TotalAmount = order.TotalAmount
        };
    }
}