using ECommerceAPI.DTOs.Order;
using ECommerceAPI.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    // ======================================================
    private readonly IAdminOrderService _service;
    public AdminOrdersController(IAdminOrderService service)
    {
        _service = service;
    }
    // ======================================================

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null
    )
    {
        var result = await _service.GetAllPagedAsync(page, pageSize, status, search);
        return Ok(result);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetById(int orderId)
    {
        var order = await _service.GetByIdAsync(orderId);

        if (order == null) return NotFound();

        return Ok(order);
    }

    [HttpPatch("{orderId}/status")]
    public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            var updated = await _service.UpdateStatusAsync(orderId, dto.Status);

            if (!updated) return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}