using ECommerceAPI.Data;
using ECommerceAPI.DTOs.Order;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services.Orders;

public class OrderService : IOrderService
{
    // ===================================
    private readonly AppDbContext _db;
    public OrderService(AppDbContext db)
    {
        _db = db;
    }
    // ===================================

    // CREATE ORDER (CHECKOUT)
    public async Task<OrderResponseDto> CreateOrderAsync(int userId)
    {
        // Obtener el carrito del usuario
        var cartItems = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (cartItems.Count == 0)
            throw new Exception("El carrito está vacío");

        // Crea la orden
        var order = new Order
        {
            UserId = userId,
            TotalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity),
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(); // Para obtener order.Id

        // Crear OrderItems
        var orderItems = cartItems.Select(item => new OrderItem
        {
            OrderId = order.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            PriceAtPurchase = item.Product.Price,
        }).ToList();

        // Vaciar carrito
        _db.CartItems.RemoveRange(cartItems);
        await _db.SaveChangesAsync();

        // REspuesta
        return new OrderResponseDto
        {
            OrderId = order.Id,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Items = [.. orderItems.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = cartItems.First(c => c.ProductId == i.ProductId).Product.Name,
                PriceAtPurchase = i.PriceAtPurchase,
                Quantity = i.Quantity
            })]
        };
    }


    // GET USER ORDERS
    public async Task<List<OrderResponseDto>> GetUserOrdersAsync(int userId)
    {
        var orders = await _db.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .ToListAsync();

        return [.. orders.Select(order => new OrderResponseDto
        {
            OrderId = order.Id,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Items = [.. order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                PriceAtPurchase = i.PriceAtPurchase,
                Quantity = i.Quantity
            })]
        })];
    }

    // GET ORDER BY ID
    public async Task<OrderResponseDto?> GetByIdAsync(int orderId, int userId)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null) return null;

        return new OrderResponseDto
        {
            OrderId = order.Id,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Items = [.. order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                PriceAtPurchase = i.PriceAtPurchase,
                Quantity = i.Quantity
            })]
        };
    }
}