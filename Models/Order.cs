namespace ECommerceAPI.Models;

public class Order
{
    public int Id { get; set; }

    // Relación con el usuario.
    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }

    // Mercado Pago.
    public string Status { get; set; } = OrderStatuses.Pending; // Pending - Paid - Preparing - Shipped - Delivered - Cancelled
    public string? MercadoPagoPreferenceId { get; set; }
    public long? MercadoPagoPaymentId { get; set; }

    public List<OrderItem> Items { get; set; } = [];
}