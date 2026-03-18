using ECommerceAPI.Data;
using ECommerceAPI.DTOs.Common;
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

    // GET USER ORDERS
    public async Task<PagedResultDto<OrderResponseDto>> GetUserOrdersPagedAsync(int userId, int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 50);

        var query = _db.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt);

        var totalItems = await query.CountAsync();

        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = orders.Select(order => new OrderResponseDto
        {
            OrderId = order.Id,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            TotalItems = order.Items.Count,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                PriceAtPurchase = i.PriceAtPurchase,
                Quantity = i.Quantity
            }).ToList(),
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResultDto<OrderResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
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
            Status = order.Status,
            TotalItems = order.Items.Count,
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