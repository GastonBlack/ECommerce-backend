
using ECommerceAPI.DTOs.Category;
using ECommerceAPI.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    // ==================================================
    private readonly ICategoryService _service;
    public CategoryController(ICategoryService service)
    {
        _service = service;
    }
    // ==================================================
    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category == null) return NotFound();

        return Ok(category);
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateDto dto)
    {
        var category = await _service.CreateAsync(dto);
        return Ok(category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
    {
        var category = await _service.UpdateAsync(id, dto);
        if (category == null) return NotFound();

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();

        return Ok(new { message = "Categoria eliminada." });
    }
}
