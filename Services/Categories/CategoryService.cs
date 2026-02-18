
using ECommerceAPI.Data;
using ECommerceAPI.DTOs.Category;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services.Categories;

public class CategoryService : ICategoryService
{
    // =======================================
    private readonly AppDbContext _db;
    public CategoryService(AppDbContext db)
    {
        _db = db;
    }
    // =======================================
    public async Task<List<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await _db.Categories.ToListAsync();

        return categories.Select(c => new CategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
        }).ToList();
    }


    public async Task<CategoryResponseDto?> GetByIdAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return null;

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
        };
    }


    public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
        };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
        };
    }


    public async Task<CategoryResponseDto?> UpdateAsync(int id, CategoryUpdateDto dto)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return null;

        category.Name = dto.Name;

        await _db.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
        };
    }


    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return false;

        _db.Categories.Remove(category);

        try
        {
            await _db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("No se puede eliminar la categoría porque tiene productos asociados.");
        }
    }
}