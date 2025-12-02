using ECommerceAPI.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class OrderController : ControllerBase
{
    // ===========================================
    private readonly IOrderService _service;
    public OrderController(IOrderService service)
    {
        _service = service;
    }
    // ===========================================

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        try
        {
            return Ok(await _service.CreateOrderAsync(userId));
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpGet("")]
    public async Task<IActionResult> GetUserOrders()
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        return Ok(await _service.GetUserOrdersAsync(userId));
    }


    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetById(int orderId)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        var order = await _service.GetByIdAsync(orderId, userId);
        if (order == null) return NotFound();

        return Ok(order);
    }
}