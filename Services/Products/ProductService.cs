
using ECommerceAPI.Data;
using ECommerceAPI.DTOs.Product;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services.Products;

public class ProductService : IProductService
{

    // ====================================
    private readonly AppDbContext _db;
    public ProductService(AppDbContext db)
    {
        _db = db;
    }
    // ====================================

    // GET ALL
    public async Task<List<ProductResponseDto>> GetAllAsync(string? sort = null)
    {
        // Primero se filtra.
        IQueryable<Product> query = _db.Products;

        switch (sort)
        {
            case "popular":
                query = query.OrderByDescending(p => p.TotalSold);
                break;

            case "price-asc":
                query = query.OrderBy(p => p.Price);
                break;

            case "price-desc":
                query = query.OrderByDescending(p => p.Price);
                break;

            case "name-asc":
                query = query.OrderBy(p => p.Name);
                break;

            case "name-desc":
                query = query.OrderByDescending(p => p.Name);
                break;
        }

        // Retorna los productos ya ordenados segun filtro.
        var products = await query.ToListAsync();
        return [.. products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId
        })];
    }

    // GET BY ID
    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return null;

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };
    }

    // CREATE 
    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };
    }

    // UPDATE PRODUCT
    public async Task<ProductResponseDto?> UpdateAsync(int id, ProductUpdateDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.ImageUrl = dto.ImageUrl;
        product.CategoryId = dto.CategoryId;

        await _db.SaveChangesAsync();

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };
    }

    // DELETE 
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return true;
    }


    // GET ALL para el admin.
    public async Task<List<ProductAdminResponseDto>> GetAllAdminAsync(string? sort = null)
    {
        IQueryable<Product> query = _db.Products;

        switch (sort)
        {
            case "popular":
                query = query.OrderByDescending(p => p.TotalSold);
                break;

            case "price-asc":
                query = query.OrderBy(p => p.Price);
                break;

            case "price-desc":
                query = query.OrderByDescending(p => p.Price);
                break;

            case "name-asc":
                query = query.OrderBy(p => p.Name);
                break;

            case "name-desc":
                query = query.OrderByDescending(p => p.Name);
                break;
        }

        var products = await query.ToListAsync();
        return [.. products.Select(p => new ProductAdminResponseDto{
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            TotalSold = p.TotalSold,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId
        })];
    }


    public async Task<ProductAdminResponseDto?> GetByIdAdminAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return null;

        return new ProductAdminResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            TotalSold = product.TotalSold,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };
    }

}