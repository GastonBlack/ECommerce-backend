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
    private readonly AppDbContext _db;
    private readonly IOptions<MercadoPagoSettings> _mp;

    public PaymentService(AppDbContext db, IOptions<MercadoPagoSettings> mp)
    {
        _db = db;
        _mp = mp;
    }

    public async Task<CreatePreferenceResponseDto> CreatePreferenceAsync(int userId)
    {
        var cartItems = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (cartItems.Count == 0)
            throw new InvalidOperationException("El carrito está vacío.");

        var totalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity);

        // Reutiliza una orden pendiente existente del usuario si todavía no fue pagada.
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o =>
                o.UserId == userId &&
                o.Status == OrderStatuses.Pending &&
                o.MercadoPagoPaymentId == null);

        if (order == null)
        {
            order = new Order
            {
                UserId = userId,
                TotalAmount = totalAmount,
                Status = OrderStatuses.Pending,
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
        }
        else
        {
            // Si ya existía una orden pendiente, la actualizamos según el carrito actual.
            order.TotalAmount = totalAmount;

            if (order.Items.Count > 0)
            {
                _db.OrderItems.RemoveRange(order.Items);
                await _db.SaveChangesAsync();
            }
        }

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

        try
        {
            var client = new PreferenceClient();
            var preference = await client.CreateAsync(preferenceRequest);

            order.MercadoPagoPreferenceId = preference.Id;
            await _db.SaveChangesAsync();

            return new CreatePreferenceResponseDto
            {
                OrderId = order.Id,
                InitPoint = preference.InitPoint,
                SandboxInitPoint = preference.SandboxInitPoint
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("MP ERROR: " + ex);
            throw new InvalidOperationException("No se pudo crear la preferencia de pago.");
        }
    }

    public async Task ProcessWebhookAsync(string? type, string? dataId, string? dataIdAlt)
    {
        var paymentIdStr = dataId ?? dataIdAlt;

        if (type != "payment" || string.IsNullOrWhiteSpace(paymentIdStr))
            return;

        MercadoPagoConfig.AccessToken = _mp.Value.AccessToken;

        var paymentClient = new PaymentClient();
        Payment payment;

        try
        {
            payment = await paymentClient.GetAsync(long.Parse(paymentIdStr));
        }
        catch
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(payment.ExternalReference))
            return;

        if (!int.TryParse(payment.ExternalReference, out var orderId))
            return;

        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return;

        // Si ya avanzó más allá del pago, no permitir que el webhook lo pise.
        if (order.Status == OrderStatuses.Paid ||
            order.Status == OrderStatuses.Preparing ||
            order.Status == OrderStatuses.Shipped ||
            order.Status == OrderStatuses.Delivered)
        {
            return;
        }

        var status = payment.Status;

        if (status == "approved")
        {
            order.Status = OrderStatuses.Paid;
            order.MercadoPagoPaymentId = payment.Id;

            var cartItems = await _db.CartItems
                .Where(c => c.UserId == order.UserId)
                .ToListAsync();

            _db.CartItems.RemoveRange(cartItems);

            foreach (var item in order.Items)
            {
                var product = await _db.Products.FindAsync(item.ProductId);
                if (product == null) continue;

                if (product.Stock >= item.Quantity)
                {
                    product.Stock -= item.Quantity;
                    product.TotalSold += item.Quantity;
                }
            }

            await _db.SaveChangesAsync();
        }
        else if (status == "rejected" || status == "cancelled")
        {
            order.Status = OrderStatuses.Cancelled;
            order.MercadoPagoPaymentId = payment.Id;
            await _db.SaveChangesAsync();
        }
        else
        {
            order.Status = OrderStatuses.Pending;
            order.MercadoPagoPaymentId = payment.Id;
            await _db.SaveChangesAsync();
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