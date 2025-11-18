using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerceAPI.Models;
using ECommerceAPI.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceAPI.Services.Jwt;

public class JwtService : IJwtService
{
    // ========================================================
    private readonly JwtSettings _jwtSettings;
    public JwtService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }
    // ========================================================

    public string GenerateToken(User user)
    {
        // 1. Clave secreta (Key) -> se convierte en bytes.
        var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var securityKey = new SymmetricSecurityKey(keyBytes);

        // 2. Credenciales de firma.
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 3. Claims = datos que van dentro del token.
        var claims = new List<Claim>
        {
            // "sub" = subject (identificador principal del token)
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            // Jti = identificador único del token.
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // 4. Crear el objeto JwtSecurityToken.
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
            signingCredentials: credentials
        );

        // 5. Convertir el token a string (lo que se manda al frontend).
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}