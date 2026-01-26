using ECommerceAPI.DTOs.User;
using ECommerceAPI.Extensions;
using ECommerceAPI.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    // ==================================================
    private readonly IUserService _service;
    public UserController(IUserService service)
    {
        _service = service;
    }
    // ==================================================

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.GetUserId();
        var me = await _service.GetMeAsync(userId);
        if (me == null) return NotFound();
        return Ok(me);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UserUpdateMeDto dto)
    {
        if (dto == null) return BadRequest(new { error = "Body inválido" });

        var userId = User.GetUserId();
        var updated = await _service.UpdateMeAsync(userId, dto);
        if (updated == null) return NotFound();

        return Ok(updated);
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDto dto)
    {
        if (dto == null) return BadRequest(new { error = "Body Inválido" });

        var userId = User.GetUserId();
        var (ok, error) = await _service.ChangePasswordAsync(userId, dto);
        if (!ok) return BadRequest(new { error });

        return Ok(new { message = "Contraseña actualizada." });
    }



}