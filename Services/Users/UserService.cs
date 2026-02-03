using ECommerceAPI.Data;
using ECommerceAPI.DTOs.User;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services.Users;

public class UserService : IUserService
{
    // ========================================
    private readonly AppDbContext _db;
    public UserService(AppDbContext db)
    {
        _db = db;
    }
    // ========================================
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.Users.FindAsync(id);
    }


    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }


    public async Task<User> CreateUserAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }


    public async Task<bool> DisableAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return false;

        // Para que no se pueda deshabilitar a los Admins.
        if (user.Rol == "Admin") return false;

        if (!user.IsDisabled)
        {
            user.IsDisabled = true;
            user.DisabledAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return true;
    }


    public async Task<bool> EnableAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return false;

        if (user.IsDisabled)
        {
            user.IsDisabled = false;
            user.DisabledAt = null;
            await _db.SaveChangesAsync();
        }

        return true;
    }


    public async Task<UserMeResponseDto?> GetMeAsync(int userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;

        return new UserMeResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Address = user.Address,
            Phone = user.Phone,
        };
    }


    public async Task<UserMeResponseDto?> UpdateMeAsync(int userId, UserUpdateMeDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;

        user.FullName = dto.FullName.Trim();
        user.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
        user.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();

        await _db.SaveChangesAsync();

        return new UserMeResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Address = user.Address,
            Phone = user.Phone,
        };
    }


    public async Task<(bool ok, string? error)> ChangePasswordAsync(int userId, UserChangePasswordDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return (false, "Usuario no encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return (false, "La contraseña actual es incorrecta");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _db.SaveChangesAsync();

        return (true, null);
    }


    public async Task<List<UserAdminListDto>> GetAllForAdminAsync()
    {
        return await _db.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserAdminListDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Rol = u.Rol,
                IsDisabled = u.IsDisabled,
                CreatedAt = u.CreatedAt,
                DisabledAt = u.DisabledAt
            }).ToListAsync();
    }



    public async Task<UserAdminListDto?> GetByIdForAdminAsync(int id)
    {
        return await _db.Users
            .Where(u => u.Id == id)
            .Select(u => new UserAdminListDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Rol = u.Rol,
                IsDisabled = u.IsDisabled,
                CreatedAt = u.CreatedAt,
                DisabledAt = u.DisabledAt
            }).FirstOrDefaultAsync();
    }
}