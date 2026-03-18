
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
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 2 || dto.Name.Trim().Length > 40)
            throw new InvalidOperationException("El nombre debe tener al menos 2 caracteres y máximo 40.");

        var normalizedName = dto.Name.Trim().ToLower();

        var alreadyExists = await _db.Categories
            .AnyAsync(c => c.Name.ToLower() == normalizedName);

        if (alreadyExists)
            throw new InvalidOperationException("Ya existe una categoría con ese nombre.");

        var category = new Category
        {
            Name = dto.Name.Trim()
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }


    public async Task<CategoryResponseDto?> UpdateAsync(int id, CategoryUpdateDto dto)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return null;

        var normalizedName = dto.Name.Trim().ToLower();

        var alreadyExists = await _db.Categories
            .AnyAsync(c => c.Id != id && c.Name.ToLower() == normalizedName);

        if (alreadyExists)
            throw new InvalidOperationException("Ya existe otra categoría con ese nombre");
        if (normalizedName.Length < 2)

            category.Name = dto.Name.Trim();

        await _db.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name
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