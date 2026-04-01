using ECommerceAPI.DTOs.Auth;
using ECommerceAPI.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Antiforgery;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    // ==============================================
    private readonly IAuthService _authService;
    private readonly IAntiforgery _antiforgery;
    public AuthController(IAuthService authService, IAntiforgery antiforgery)
    {
        _authService = authService;
        _antiforgery = antiforgery;
    }

    private CookieOptions BuildTokenCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = HttpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        };
    }

    private CookieOptions BuildUserCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = HttpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        };
    }

    // =====================
    // CSRF
    // =====================
    private void RefreshCsrfToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
            new CookieOptions
            {
                HttpOnly = false, // Para que Front la lea.
                Secure = HttpContext.Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
    }

    [HttpGet("csrf")]
    [AllowAnonymous]
    public IActionResult GetCsrfToken()
    {
        RefreshCsrfToken();
        return Ok(new { message = "CSRF token generado." });
    }
    // ==============================================

    // =====================
    // REGISTER
    // =====================
    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            Response.Cookies.Append("token", result.Token, BuildTokenCookieOptions());
            Response.Cookies.Append("userName", result.FullName, BuildUserCookieOptions());

            RefreshCsrfToken();

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
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);

            Response.Cookies.Append("token", result.Token, BuildTokenCookieOptions());
            Response.Cookies.Append("userName", result.FullName, BuildUserCookieOptions());

            RefreshCsrfToken();

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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        var userId = int.Parse(userIdClaim);
        var user = await _authService.GetMeAsync(userId);

        if (user == null)
            return Unauthorized();

        RefreshCsrfToken();

        return Ok(user);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Opciones para borrar cookies.
        var deleteOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = HttpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            Path = "/"
        };

        // Borrar JWT y nombre de usuario
        Response.Cookies.Append("token", "", deleteOptions);
        Response.Cookies.Append("userName", "", deleteOptions);

        // Borrar Cookies de Antiforgery
        // Borra la interna de .NET
        Response.Cookies.Append(".AspNetCore.Antiforgery", "", deleteOptions);

        // Borra la que lee Axios (HttpOnly falso).
        var deleteOptionsAxios = new CookieOptions
        {
            HttpOnly = false,
            Secure = HttpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            Path = "/"
        };
        Response.Cookies.Append("XSRF-TOKEN", "", deleteOptionsAxios);

        return Ok(new { message = "Logout exitoso y tokens limpiados." });
    }
}