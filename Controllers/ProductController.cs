using ECommerceAPI.DTOs.Product;
using ECommerceAPI.Services.ImageUpload;
using ECommerceAPI.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProductController : ControllerBase
{
    // ==================================================
    private readonly IProductService _service;
    public ProductController(IProductService service)
    {
        _service = service;
    }
    // ==================================================

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sort = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? search = null
    )
    {
        return Ok(await _service.GetPagedAsync(page, pageSize, sort, categoryId, minPrice, maxPrice, search));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var product = await _service.CreateAsync(dto);
        return Ok(product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        var product = await _service.UpdateAsync(id, dto);
        if (product == null) return NotFound();

        return Ok(product);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return Ok(new { message = "Producto eliminado." });
    }

    [HttpPost("upload-image")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromServices] ICloudinaryService service)
    {
        try
        {
            var url = await service.UploadImageAsync(file);
            return Ok(new { imageUrl = url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAdmin(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sort = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? search = null
    )
    {
        return Ok(await _service.GetPagedAdminAsync(page, pageSize, sort, categoryId, minPrice, maxPrice, search));
    }
}