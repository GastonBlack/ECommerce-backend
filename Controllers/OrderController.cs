using ECommerceAPI.Services.Orders;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class OrderController : ControllerBase
{
    // ===========================================
    private readonly IOrderService _service;
    public OrderController(IOrderService service)
    {
        _service = service;
    }
    // ===========================================

    [HttpPost("{userId}/checkout")]
    public async Task<IActionResult> Checkout(int userId)
    {
        try
        {
            return Ok(await _service.CreateOrderAsync(userId));
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserOrders(int userId)
    {
        return Ok(await _service.GetUserOrdersAsync(userId));
    }


    [HttpGet("{userId}/{orderId}")]
    public async Task<IActionResult> GetById(int userId, int orderId)
    {
        var order = await _service.GetByIdAsync(orderId, userId);
        if (order == null) return NotFound();

        return Ok(order);
    }
}