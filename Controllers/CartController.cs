using ECommerceAPI.DTOs.Cart;
using ECommerceAPI.Extensions;
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
        var userId = User.GetUserId();
        return Ok(await _service.GetUserCartAsync(userId));
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] CartAddDto dto)
    {
        if (dto == null)
            return BadRequest(new { error = "Body inválido (dto es null)." });

        try
        {
            var userId = User.GetUserId();
            return Ok(await _service.AddToCartAsync(userId, dto));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                code = "INSUFFICIENT_STOCK",
                error = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                code = "PRODUCT_NOT_FOUND",
                error = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                code = "INVALID_QUANTITY",
                error = ex.Message
            });
        }
    }

    [HttpPut("update/{cartItemId}")]
    public async Task<IActionResult> Update(int cartItemId, CartUpdateDto dto)
    {
        try
        {
            var userId = User.GetUserId();

            var result = await _service.UpdateQuantityAsync(cartItemId, dto, userId);
            if (result == null)
            {
                return NotFound(new
                {
                    code = "CART_ITEM_NOT_FOUND",
                    error = "Item no encontrado."
                });
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                code = "INSUFFICIENT_STOCK",
                error = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                code = "INVALID_QUANTITY",
                error = ex.Message
            });
        }
    }

    [HttpDelete("remove/{cartItemId}")]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var userId = User.GetUserId();

        var removed = await _service.RemoveItemAsync(cartItemId, userId);
        if (!removed)
        {
            return NotFound(new
            {
                code = "CART_ITEM_NOT_FOUND",
                error = "Item no encontrado."
            });
        }

        return Ok(new { message = "Item eliminado" });
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        var userId = User.GetUserId();

        var cleared = await _service.ClearCartAsync(userId);
        if (!cleared)
        {
            return NotFound(new
            {
                code = "EMPTY_CART",
                error = "El carrito ya está vacío."
            });
        }

        return Ok(new { message = "Carrito vaciado" });
    }
}
