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

    private bool IsDevelopment()
    {
        var env = Environment.GetEnvironmentVariable("APP_ENV");
        return string.Equals(env, "development", StringComparison.OrdinalIgnoreCase);
    }

    private CookieOptions BuildTokenCookieOptions()
    {
        var isDev = IsDevelopment();

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDev,
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        };
    }

    private CookieOptions BuildUserCookieOptions()
    {
        var isDev = IsDevelopment();

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDev,
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        };
    }

    private string GenerateCsrfToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        return tokens.RequestToken!;
    }

    [HttpGet("csrf")]
    [AllowAnonymous]
    public IActionResult GetCsrfToken()
    {
        var csrfToken = GenerateCsrfToken();
        return Ok(new { csrfToken });
    }
    // ==============================================
    
    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);

            Response.Cookies.Append("token", result.Token, BuildTokenCookieOptions());
            Response.Cookies.Append("userName", result.FullName, BuildUserCookieOptions());

            return Ok(new
            {
                fullName = result.FullName,
                email = result.Email,
                userId = result.UserId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);

            Response.Cookies.Append("token", result.Token, BuildTokenCookieOptions());
            Response.Cookies.Append("userName", result.FullName, BuildUserCookieOptions());

            return Ok(new
            {
                fullName = result.FullName,
                email = result.Email,
                userId = result.UserId
            });
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

        return Ok(user);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var isDev = IsDevelopment();

        var deleteHttpOnlyCookie = new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDev,
            SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            Path = "/"
        };

        Response.Cookies.Append("token", "", deleteHttpOnlyCookie);
        Response.Cookies.Append("userName", "", deleteHttpOnlyCookie);
        Response.Cookies.Append(".AspNetCore.Antiforgery", "", deleteHttpOnlyCookie);

        return Ok(new { message = "Logout exitoso y tokens limpiados." });
    }
}