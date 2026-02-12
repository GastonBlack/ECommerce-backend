using ECommerceAPI.DTOs.Product;

namespace ECommerceAPI.Services.Products;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync(string? sort = null);
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto);
    Task<ProductResponseDto?> UpdateAsync(int id, ProductUpdateDto dto);
    Task<bool> DeleteAsync(int id);

    Task<List<ProductAdminResponseDto>> GetAllAdminAsync(string? sort = null);
    Task<ProductAdminResponseDto?> GetByIdAdminAsync(int id);
}