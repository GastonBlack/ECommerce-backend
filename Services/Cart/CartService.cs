using ECommerceAPI.Data;
using ECommerceAPI.DTOs.Cart;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services.Cart;

public class CartService : ICartService
{
    // =================================
    private readonly AppDbContext _db;
    public CartService(AppDbContext db)
    {
        _db = db;
    }
    // =================================

    public async Task<List<CartItemResponseDto>> GetUserCartAsync(int userId)
    {
        var cartItems = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return cartItems.Select(item => new CartItemResponseDto
        {
            CartItemId = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            ImageUrl = item.Product.ImageUrl,
            Price = item.Product.Price,
            Quantity = item.Quantity
        }).ToList();
    }


    public async Task<CartItemResponseDto> AddToCartAsync(int userId, CartAddDto dto)
    {
        // Si ya existe el item en el carrito se le incrementa la cantidad
        var existing = await _db.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

        if (existing != null)
        {
            existing.Quantity += dto.Quantity;
            await _db.SaveChangesAsync();

            var item = await _db.Products.FindAsync(existing.ProductId);
            return new CartItemResponseDto
            {
                CartItemId = existing.Id,
                ProductId = existing.ProductId,
                ProductName = item!.Name,
                ImageUrl = item.ImageUrl,
                Price = item.Price,
                Quantity = existing.Quantity
            };
        }

        // Si no existe, crea uno nuevo.
        var newItem = new CartItem
        {
            UserId = userId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity
        };

        _db.CartItems.Add(newItem);
        await _db.SaveChangesAsync();

        var product = await _db.Products.FindAsync(dto.ProductId);
        return new CartItemResponseDto
        {
            CartItemId = newItem.Id,
            ProductId = newItem.ProductId,
            ProductName = product!.Name,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            Quantity = newItem.Quantity
        };
    }


    public async Task<CartItemResponseDto?> UpdateQuantityAsync(int cartItemId, CartUpdateDto dto, int userId)
    {
        var item = await _db.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

        if (item == null)
            return null;

        item.Quantity = dto.Quantity;
        await _db.SaveChangesAsync();

        return new CartItemResponseDto
        {
            CartItemId = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            ImageUrl = item.Product.ImageUrl,
            Price = item.Product.Price,
            Quantity = item.Quantity
        };
    }


    public async Task<bool> RemoveItemAsync(int cartItemId, int userId)
    {
        var item = await _db.CartItems
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

        if (item == null) return false;

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }


    public async Task<bool> ClearCartAsync(int userId)
    {
        var items = await _db.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!items.Any()) return false;

        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync();
        return true;
    }
}