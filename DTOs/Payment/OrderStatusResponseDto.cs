namespace ECommerceAPI.DTOs.Payment;

public class OrderStatusResponseDto
{
    public int OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}