using ECommerceAPI.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]

public class AdminUsersController : ControllerBase
{
    // ===========================================
    private readonly IUserService _service;
    public AdminUsersController(IUserService service)
    {
        _service = service;
    }
    // ===========================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllForAdminAsync());
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _service.GetByIdForAdminAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }


    [HttpPatch("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {
        var ok = await _service.DisableAsync(id);
        if (!ok) return BadRequest(new { error = "No se pudo deshabilitar (no existe o es Admin)." });

        return Ok(new { message = "Usuario deshabilitado." });
    }


    [HttpPatch("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {
        var ok = await _service.EnableAsync(id);
        if (!ok) return NotFound();

        return Ok(new { message = "Usuario habilitado." });
    }
}