using ECommerceAPI.DTOs.Cart;
using ECommerceAPI.Services.Cart;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service)
    {
        _service = service;
    }

    // Obtener carrito del usuario
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCart(int userId)
    {
        return Ok(await _service.GetUserCartAsync(userId));
    }

    // Agregar al carrito
    [HttpPost("{userId}/add")]
    public async Task<IActionResult> Add(int userId, CartAddDto dto)
    {
        return Ok(await _service.AddToCartAsync(userId, dto));
    }

    // Actualizar cantidad
    [HttpPut("{userId}/update/{cartItemId}")]
    public async Task<IActionResult> Update(int userId, int cartItemId, CartUpdateDto dto)
    {
        if (dto.Quantity <= 0)
            return BadRequest(new { error = "La cantidad debe ser mayor a 0" });

        var result = await _service.UpdateQuantityAsync(cartItemId, dto, userId);
        if (result == null) return NotFound();

        return Ok(result);
    }

    // Eliminar item
    [HttpDelete("{userId}/remove/{cartItemId}")]
    public async Task<IActionResult> Remove(int userId, int cartItemId)
    {
        var removed = await _service.RemoveItemAsync(cartItemId, userId);
        if (!removed) return NotFound();

        return Ok(new { message = "Item eliminado" });
    }

    // Vaciar carrito
    [HttpDelete("{userId}/clear")]
    public async Task<IActionResult> Clear(int userId)
    {
        var cleared = await _service.ClearCartAsync(userId);
        if (!cleared) return NotFound();

        return Ok(new { message = "Carrito vaciado" });
    }
}
