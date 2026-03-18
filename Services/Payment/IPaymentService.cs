using ECommerceAPI.DTOs.Payment;

namespace ECommerceAPI.Services.Payments;

public interface IPaymentService
{
    Task<CreatePreferenceResponseDto> CreatePreferenceAsync(int userId);
    Task ProcessWebhookAsync(string? type, string? dataId, string? dataIdAlt);
    Task<OrderStatusResponseDto> GetOrderStatusAsync(int orderId, int userId);
}