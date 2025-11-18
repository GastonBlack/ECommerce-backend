using ECommerceAPI.Models;

namespace ECommerceAPI.Services.Jwt;

public interface IJwtService
{
    string GenerateToken(User user);
}