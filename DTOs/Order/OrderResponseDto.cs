namespace ECommerceAPI.DTOs.Order;

public class OrderResponseDto
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<OrderItemDto> Items { get; set; } = [];
}