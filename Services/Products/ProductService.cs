
using ECommerceAPI.Data;
using ECommerceAPI.DTOs.Common;
using ECommerceAPI.DTOs.Product;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerceAPI.Services.Products;

public class ProductService : IProductService
{

    // ====================================
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    // Funciones para obtener e incrementar la versión global del cache de productos.
    private int GetProductsCacheVersion()
    {
        return _cache.GetOrCreate(ProductsCacheVersionKey, entry =>
        {
            entry.Priority = CacheItemPriority.NeverRemove;
            return 1;
        });
    }

    private void BumpProductsCacheVersion()
    {
        var currentVersion = GetProductsCacheVersion();
        _cache.Set(ProductsCacheVersionKey, currentVersion + 1);
    }

    // Builders de keys de cache que incluyen la versión global.
    // ***********************
    private const string ProductsCacheVersionKey = "products:cache:version";
    private string BuildPagedProductsCacheKey(
        int page,
        int pageSize,
        string? sort,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? search)
    {
        var version = GetProductsCacheVersion();
        return $"products:v={version}:p={page}:ps={pageSize}:sort={sort ?? "null"}:cat={categoryId?.ToString() ?? "null"}:min={minPrice?.ToString() ?? "null"}:max={maxPrice?.ToString() ?? "null"}:search={(search ?? "").Trim().ToLower()}";
    }

    private string BuildAllProductsCacheKey(string? sort)
    {
        var version = GetProductsCacheVersion();
        return $"products:v={version}:all:sort={sort ?? "null"}";
    }

    private string BuildProductByIdCacheKey(int id)
    {
        var version = GetProductsCacheVersion();
        return $"products:v={version}:id={id}";
    }
    // ************************************

    public ProductService(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }
    // ====================================

    public async Task<PagedResultDto<ProductResponseDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? sort,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? search)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : Math.Min(pageSize, 100);

        var cacheKey = BuildPagedProductsCacheKey(
            page,
            pageSize,
            sort,
            categoryId,
            minPrice,
            maxPrice,
            search
        );

        if (_cache.TryGetValue(cacheKey, out PagedResultDto<ProductResponseDto>? cachedResult))
        {
            return cachedResult!;
        }

        IQueryable<Product> query = _db.Products;

        // Ordena por sort recibido.
        if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);
        if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(s));
        }
        query = sort switch
        {
            "popular" => query.OrderByDescending(p => p.TotalSold),
            "price-asc" => query.OrderBy(p => p.Price),
            "price-desc" => query.OrderByDescending(p => p.Price),
            "name-asc" => query.OrderBy(p => p.Name),
            "name-desc" => query.OrderByDescending(p => p.Name),
            _ => query.OrderBy(p => p.Id),
        };

        var totalItems = await query.CountAsync();

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock - p.ReservedStock,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var result = new PagedResultDto<ProductResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _cache.Set(cacheKey, result, cacheOptions);

        return result;
    }


    // GET ALL
    public async Task<List<ProductResponseDto>> GetAllAsync(string? sort = null)
    {
        var cacheKey = BuildAllProductsCacheKey(sort);

        if (_cache.TryGetValue(cacheKey, out List<ProductResponseDto>? cachedProducts))
        {
            return cachedProducts!;
        }

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

        var result = products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock - p.ReservedStock,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId
        }).ToList();

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _cache.Set(cacheKey, result, cacheOptions);
        return result;
    }

    // GET BY ID
    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var cacheKey = BuildProductByIdCacheKey(id);

        if (_cache.TryGetValue(cacheKey, out ProductResponseDto? cachedProduct))
        {
            return cachedProduct;
        }

        var product = await _db.Products.FindAsync(id);
        if (product == null) return null;

        var result = new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock - product.ReservedStock,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _cache.Set(cacheKey, result, cacheOptions);
        return result;
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
        BumpProductsCacheVersion();

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
        BumpProductsCacheVersion();

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
        BumpProductsCacheVersion();

        return true;
    }

    public async Task<PagedResultDto<ProductAdminResponseDto>> GetPagedAdminAsync(
        int page,
        int pageSize,
        string? sort,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? search
    )
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : Math.Min(pageSize, 25); // admin: máximo 25

        IQueryable<Product> query = _db.Products;

        if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);
        if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(s));
        }

        query = sort switch
        {
            "popular" => query.OrderByDescending(p => p.TotalSold),

            "stock-asc" => query.OrderBy(p => p.Stock).ThenBy(p => p.Id),
            "stock-desc" => query.OrderByDescending(p => p.Stock).ThenBy(p => p.Id),

            "price-asc" => query.OrderBy(p => p.Price),
            "price-desc" => query.OrderByDescending(p => p.Price),

            "name-asc" => query.OrderBy(p => p.Name),
            "name-desc" => query.OrderByDescending(p => p.Name),

            _ => query.OrderBy(p => p.Id),
        };

        var totalItems = await query.CountAsync();

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = products.Select(p => new ProductAdminResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock - p.ReservedStock,
            ReservedStock = p.ReservedStock,
            TotalSold = p.TotalSold,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResultDto<ProductAdminResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }
}