using ECommerceAPI.Data;
using ECommerceAPI.DTOs.Common;
using ECommerceAPI.DTOs.Order;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services.Orders;

public class AdminOrderService : IAdminOrderService
{
    // =========================================
    private readonly AppDbContext _db;
    public AdminOrderService(AppDbContext db)
    {
        _db = db;
    }
    // =========================================

    public async Task<PagedResultDto<AdminOrderResponseDto>> GetAllPagedAsync(
        int page,
        int pageSize,
        string? status = null,
        string? search = null
    )
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 50);

        var query = _db.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(o => o.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();

            query = query.Where(o =>
                o.User.FullName.ToLower().Contains(term) ||
                o.User.Email.ToLower().Contains(term) ||
                o.Id.ToString().Contains(term)
            );
        }

        query = query.OrderByDescending(o => o.CreatedAt);

        var totalItems = await query.CountAsync();

        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = orders.Select(order => new AdminOrderResponseDto
        {
            OrderId = order.Id,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            TotalItems = order.Items.Count,
            CreatedAt = order.CreatedAt,
            UserId = order.UserId,
            UserName = order.User.FullName,
            UserEmail = order.User.Email,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                PriceAtPurchase = i.PriceAtPurchase,
                Quantity = i.Quantity
            }).ToList()
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResultDto<AdminOrderResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<AdminOrderResponseDto?> GetByIdAsync(int orderId)
    {
        var order = await _db.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return null;

        return new AdminOrderResponseDto
        {
            OrderId = order.Id,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            TotalItems = order.Items.Count,
            CreatedAt = order.CreatedAt,
            UserId = order.UserId,
            UserName = order.User.FullName,
            UserEmail = order.User.Email,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                PriceAtPurchase = i.PriceAtPurchase,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task<bool> UpdateStatusAsync(int orderId, string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("El estado es obligatorio.");

        var normalized = NormalizeStatus(newStatus);

        var validStatuses = new[]
        {
            OrderStatuses.Pending,
            OrderStatuses.Paid,
            OrderStatuses.Preparing,
            OrderStatuses.Shipped,
            OrderStatuses.Delivered,
            OrderStatuses.Cancelled
        };

        if (!validStatuses.Contains(normalized))
            throw new ArgumentException("Estado inválido.");

        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return false;

        if (!OrderStatusHelper.CanTransition(order.Status, normalized))
            throw new InvalidOperationException(
                $"No se puede cambiar una orden de {order.Status} a {normalized}."
            );

        order.Status = normalized;
        await _db.SaveChangesAsync();

        return true;
    }

    private static string NormalizeStatus(string status)
    {
        var s = status.Trim().ToLower();

        return s switch
        {
            "pending" => OrderStatuses.Pending,
            "paid" => OrderStatuses.Paid,
            "preparing" => OrderStatuses.Preparing,
            "shipped" => OrderStatuses.Shipped,
            "delivered" => OrderStatuses.Delivered,
            "cancelled" => OrderStatuses.Cancelled,
            _ => status.Trim()
        };
    }
}