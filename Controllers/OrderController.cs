using ECommerceAPI.Extensions;
using ECommerceAPI.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    // =====================================================
    private readonly IOrderService _service;
    public OrderController(IOrderService service)
    {
        _service = service;
    }
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var userId = User.GetUserId();

        var result = await _service.GetUserOrdersPagedAsync(
            userId,
            page,
            pageSize
        );

        return Ok(result);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetById(int orderId)
    {
        var userId = User.GetUserId();

        var order = await _service.GetByIdAsync(orderId, userId);

        if (order == null) return NotFound();

        return Ok(order);
    }
}