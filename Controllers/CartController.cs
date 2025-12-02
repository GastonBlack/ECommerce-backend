using ECommerceAPI.DTOs.Cart;
using ECommerceAPI.Services.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class CartController : ControllerBase
{
    // ===========================================
    private readonly ICartService _service;
    public CartController(ICartService service)
    {
        _service = service;
    }
    // ===========================================

    // Obtener carrito del usuario
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);
        
        return Ok(await _service.GetUserCartAsync(userId));
    }

    // Agregar al carrito
    [HttpPost("add")]
    public async Task<IActionResult> Add(CartAddDto dto)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        return Ok(await _service.AddToCartAsync(userId, dto));
    }

    // Actualizar cantidad
    [HttpPut("update/{cartItemId}")]
    public async Task<IActionResult> Update(int cartItemId, CartUpdateDto dto)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        if (dto.Quantity <= 0)
            return BadRequest(new { error = "La cantidad debe ser mayor a 0" });

        var result = await _service.UpdateQuantityAsync(cartItemId, dto, userId);
        if (result == null) return NotFound();

        return Ok(result);
    }

    // Eliminar item
    [HttpDelete("remove/{cartItemId}")]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        var removed = await _service.RemoveItemAsync(cartItemId, userId);
        if (!removed) return NotFound();

        return Ok(new { message = "Item eliminado" });
    }

    // Vaciar carrito
    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        var cleared = await _service.ClearCartAsync(userId);
        if (!cleared) return NotFound();

        return Ok(new { message = "Carrito vaciado" });
    }
}
