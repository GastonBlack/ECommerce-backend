using ECommerceAPI.DTOs.Common;
using ECommerceAPI.DTOs.Order;

namespace ECommerceAPI.Services.Orders;

public interface IOrderService
{
    Task<PagedResultDto<OrderResponseDto>> GetUserOrdersPagedAsync(int userId, int page, int pageSize);
    Task<OrderResponseDto?> GetByIdAsync(int orderId, int userId);
}