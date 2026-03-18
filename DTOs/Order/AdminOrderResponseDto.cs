namespace ECommerceAPI.DTOs.Order;

public class AdminOrderResponseDto
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    public List<OrderItemDto> Items { get; set; } = [];
}