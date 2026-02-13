using ECommerceAPI.DTOs.Auth;
using ECommerceAPI.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    // ==============================================
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _env;
    public AuthController(IAuthService authService, IWebHostEnvironment env)
    {
        _authService = authService;
        _env = env;
    }

    private CookieOptions BuildTokenCookieOptions()
    {
        var isProd = !_env.IsDevelopment();// localhost = dev

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = isProd,
            SameSite = isProd ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        };
    }

    private CookieOptions BuildUserCookieOptions()
    {
        var isProd = !_env.IsDevelopment();

        return new CookieOptions
        {
            HttpOnly = false,
            Secure = isProd,
            SameSite = isProd ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        };
    }
    // ==============================================

    // =====================
    // REGISTER
    // =====================
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            Response.Cookies.Append("token", result.Token, BuildTokenCookieOptions());
            Response.Cookies.Append("userName", result.FullName, BuildUserCookieOptions());

            return Ok(new { fullName = result.FullName, email = result.Email, userId = result.UserId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // =====================
    // LOGIN
    // =====================
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);

            Response.Cookies.Append("token", result.Token, BuildTokenCookieOptions());
            Response.Cookies.Append("userName", result.FullName, BuildUserCookieOptions());

            return Ok(new { fullName = result.FullName, email = result.Email, userId = result.UserId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("verify")]
    public async Task<IActionResult> Verify()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);
        var user = await _authService.GetMeAsync(userId);

        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(user);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("userName", new CookieOptions { Path = "/" });
        return Ok();
    }
}