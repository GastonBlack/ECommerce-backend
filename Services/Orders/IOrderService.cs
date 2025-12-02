using ECommerceAPI.DTOs.Order;

namespace ECommerceAPI.Services.Orders;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(int userId);
    Task<List<OrderResponseDto>> GetUserOrdersAsync(int userId);
    Task<OrderResponseDto?> GetByIdAsync(int orderId, int userId);
}