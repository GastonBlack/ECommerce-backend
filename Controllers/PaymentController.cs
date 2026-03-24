using ECommerceAPI.Extensions;
using ECommerceAPI.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    // =====================================================
    private readonly IPaymentService _service;
    public PaymentsController(IPaymentService service)
    {
        _service = service;
    }
    // =====================================================

    [HttpPost("create-preference")]
    [EnableRateLimiting("payment")]
    public async Task<IActionResult> CreatePreference()
    {
        try
        {
            var userId = User.GetUserId();
            var result = await _service.CreatePreferenceAsync(userId);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Ocurrió un error interno al crear la preferencia." });
        }
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(
        [FromQuery] string? type,
        [FromQuery(Name = "data.id")] string? dataId,
        [FromQuery(Name = "data_id")] string? dataIdAlt)
    {
        await _service.ProcessWebhookAsync(type, dataId, dataIdAlt);
        return Ok();
    }

    [HttpGet("order-status/{orderId:int}")]
    public async Task<IActionResult> GetOrderStatus(int orderId)
    {
        try
        {
            var userId = User.GetUserId();
            var result = await _service.GetOrderStatusAsync(orderId, userId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}