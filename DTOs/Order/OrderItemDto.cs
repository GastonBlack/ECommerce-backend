using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.Order;

public class OrderItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Required, MinLength(2), MaxLength(60)]
    public string ProductName { get; set; } = null!;

    [Required, Range(0.1, 1000000)]
    public decimal PriceAtPurchase { get; set; }

    [Required, Range(1, 999)]
    public int Quantity { get; set; }
}