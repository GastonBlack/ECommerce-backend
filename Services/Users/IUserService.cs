using ECommerceAPI.DTOs.Common;
using ECommerceAPI.DTOs.User;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services.Users;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);           // Para los endpoints que utilizan datos del usuario.
    Task<User?> GetByEmailAsync(string email);  // Encuentra a un usuario con email.
    Task<User> CreateUserAsync(User user);      // Crea a un usuario y lo añade a la BD.
    Task<bool> DisableAsync(int id);            // Administración.
    Task<bool> EnableAsync(int id);             // Administración.

    Task<UserMeResponseDto?> GetMeAsync(int userId);                                                /////////////////////////////
    Task<UserMeResponseDto?> UpdateMeAsync(int userId, UserUpdateMeDto dto);                        // Para página de profile. //
    Task<(bool ok, string? error)> ChangePasswordAsync(int userId, UserChangePasswordDto dto);      /////////////////////////////

    // Administración.
    Task<List<UserAdminListDto>> GetAllForAdminAsync();
    Task<UserAdminListDto?> GetByIdForAdminAsync(int id);
    Task<PagedResultDto<UserAdminListDto>> GetPagedForAdminAsync(int page, int pageSize, string? search, bool includeDisabled);
}