using ECommerceAPI.DTOs.Cart;

namespace ECommerceAPI.Services.Cart;

public interface ICartService
{
    Task<List<CartItemResponseDto>> GetUserCartAsync(int userId);                                   // Obtener carrito de un usuario.
    Task<CartItemResponseDto> AddToCartAsync(int userId, CartAddDto dto);                           // Agregar un producto al carrito de un usuario.
    Task<CartItemResponseDto?> UpdateQuantityAsync(int cartItemId, CartUpdateDto dto, int userId);  // ACtualizar la cantidad de un producto ya existente en el carrito de un usuario.
    Task<bool> RemoveItemAsync(int cartItemId, int userId);                                         // Eliminar un item del carrito.
    Task<bool> ClearCartAsync(int userId);                                                          // Eliminar todos los items del carrito del usuario.
}