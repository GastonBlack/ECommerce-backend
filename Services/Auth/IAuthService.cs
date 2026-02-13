using ECommerceAPI.DTOs.Auth;
using ECommerceAPI.DTOs.User;

namespace ECommerceAPI.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(UserRegisterDto dto);
    Task<AuthResponseDto> LoginAsync(UserLoginDto dto);
    Task<UserMeResponseDto?> GetMeAsync(int userId);
}