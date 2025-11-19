using ECommerceAPI.DTOs.Auth;

namespace ECommerceAPI.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(UserRegisterDto dto);
    Task<AuthResponseDto> LoginAsync(UserLoginDto dto);
}