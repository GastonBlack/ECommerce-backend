using ECommerceAPI.Data;
using ECommerceAPI.Extensions;
using ECommerceAPI.Models;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    // ==================================================
    private readonly AppDbContext _db;
    private readonly IOptions<MercadoPagoSettings> _mp;

    public PaymentsController(AppDbContext db, IOptions<MercadoPagoSettings> mp)
    {
        _db = db;
        _mp = mp;
    }
    // ==================================================

    // Crear preferencia (crea Order Pending + items).
    [HttpPost("create-preference")]
    public async Task<IActionResult> CreatePreference()
    {
        Console.WriteLine("CREATE PREFERENCE HIT");
        try
        {
            var userId = User.GetUserId();

            var cartItems = await _db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
                return BadRequest(new { error = "El carrito está vacío" });

            // 1) Crear Order Pending
            var order = new Order
            {
                UserId = userId,
                TotalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity),
                Status = "Pending",
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync(); // ya tenemos order.Id

            // 2) Crear OrderItems con OrderId
            var orderItems = cartItems.Select(i => new OrderItem
            {
                OrderId = order.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                PriceAtPurchase = i.Product.Price
            }).ToList();

            _db.OrderItems.AddRange(orderItems);
            await _db.SaveChangesAsync();

            // 3) Config MP
            MercadoPagoConfig.AccessToken = _mp.Value.AccessToken;

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
                    Success = "https://ecommerce-git-main-gastonreds-projects.vercel.app//checkout?status=success",
                    Failure = "https://ecommerce-git-main-gastonreds-projects.vercel.app//checkout?status=failure",
                    Pending = "https://ecommerce-git-main-gastonreds-projects.vercel.app//checkout?status=pending",
                },
                AutoReturn = "approved",
                NotificationUrl = $"{_mp.Value.WebhookBaseUrl}/api/payments/webhook"

            };

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(preferenceRequest);

            order.MercadoPagoPreferenceId = preference.Id;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                orderId = order.Id,
                initPoint = preference.InitPoint,
                sandboxInitPoint = preference.SandboxInitPoint
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("MP ERROR: " + ex);
            return BadRequest(new { error = ex.Message });
        }
    }


    // Webhook: MercadoPago llama aca cuando cambia estado de pago.
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(
    [FromQuery] string? type,
    [FromQuery(Name = "data.id")] string? dataId,
    [FromQuery(Name = "data_id")] string? dataIdAlt)
    {
        var paymentIdStr = dataId ?? dataIdAlt;

        if (type != "payment" || string.IsNullOrWhiteSpace(paymentIdStr))
            return Ok();

        MercadoPagoConfig.AccessToken = _mp.Value.AccessToken;

        var paymentClient = new PaymentClient();
        Payment payment;
        try
        {
            payment = await paymentClient.GetAsync(long.Parse(paymentIdStr));
        }
        catch
        {
            return Ok();
        }

        if (string.IsNullOrWhiteSpace(payment.ExternalReference))
            return Ok();

        if (!int.TryParse(payment.ExternalReference, out var orderId))
            return Ok();

        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return Ok();

        if (order.Status == "Paid") return Ok();

        var status = payment.Status; // Approved, pending, rejected...

        if (status == "approved")
        {
            order.Status = "Paid";
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
            order.Status = "Cancelled";
            order.MercadoPagoPaymentId = payment.Id;
            await _db.SaveChangesAsync();
        }
        else
        {
            order.Status = "Pending";
            order.MercadoPagoPaymentId = payment.Id;
            await _db.SaveChangesAsync();
        }

        return Ok();
    }


    // Endpoint para que el frontend consulte estado de una orden.
    [HttpGet("order-status/{orderId:int}")]
    public async Task<IActionResult> GetOrderStatus(int orderId)
    {
        var userId = User.GetUserId();

        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        if (order == null) return NotFound();

        return Ok(new
        {
            orderId = order.Id,
            status = order.Status,
            totalAmount = order.TotalAmount
        });
    }
}