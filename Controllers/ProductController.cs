using CloudinaryDotNet;
using ECommerceAPI.DTOs.Product;
using ECommerceAPI.Services.ImageUpload;
using ECommerceAPI.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
// Por ahora cualquiera puede crear/borrar/actualizar productos, falta implementar roles y despues con Authorize(Roles = "Admin).

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
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var product = await _service.CreateAsync(dto);
        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        var product = await _service.UpdateAsync(id, dto);
        if (product == null) return NotFound();

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return Ok(new { message = "Producto eliminado." });
    }

    [HttpPost("upload-image")]
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
}
