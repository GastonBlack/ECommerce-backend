using ECommerceAPI.DTOs.Common;
using ECommerceAPI.DTOs.Order;

namespace ECommerceAPI.Services.Orders;

public interface IAdminOrderService
{
    Task<PagedResultDto<AdminOrderResponseDto>> GetAllPagedAsync(
        int page,
        int pageSize,
        string? status = null,
        string? search = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null
    );

    Task<AdminOrderResponseDto?> GetByIdAsync(int orderId);
    Task<bool> UpdateStatusAsync(int orderId, string newStatus);
}