using ECommerceAPI.DTOs.Common;
using ECommerceAPI.DTOs.Product;

namespace ECommerceAPI.Services.Products;

public interface IProductService
{
    Task<PagedResultDto<ProductResponseDto>> GetPagedAsync(int page, int pageSize, string? sort, int? categoryId, decimal? minPrice, decimal? maxPrice, string? search);

    Task<List<ProductResponseDto>> GetAllAsync(string? sort = null);
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto);
    Task<ProductResponseDto?> UpdateAsync(int id, ProductUpdateDto dto);
    Task<bool> DeleteAsync(int id);

    Task<PagedResultDto<ProductAdminResponseDto>> GetPagedAdminAsync(int page, int pageSize, string? sort, int? categoryId, decimal? minPrice, decimal? maxPrice, string? search);
}