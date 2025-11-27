using ECommerceAPI.DTOs.Product;

namespace ECommerceAPI.Services.Products;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto);
    Task<ProductResponseDto?> UpdateAsync(int id, ProductUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}