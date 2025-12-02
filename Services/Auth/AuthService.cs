using System.Text.RegularExpressions;
using BCrypt.Net;
using ECommerceAPI.Data;
using ECommerceAPI.DTOs.Auth;
using ECommerceAPI.Models;
using ECommerceAPI.Services.Jwt;
using ECommerceAPI.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services.Auth;

public class AuthService : IAuthService
{
    // =================================================================
    private readonly AppDbContext _db;
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public AuthService(AppDbContext db, IJwtService jwtService, IUserService userService)
    {
        _db = db;
        _jwtService = jwtService;
        _userService = userService;
    }
    // =================================================================

    // ================
    //  REGISTER
    // ================
    public async Task<AuthResponseDto> RegisterAsync(UserRegisterDto dto)
    {
        var existing = await _userService.GetByEmailAsync(dto.Email);
        if (existing != null)
            throw new Exception("El email ya está registrado.");

        // VALIDACIONES EXTRAS
        if (string.IsNullOrWhiteSpace(dto.FullName) || dto.FullName.Length < 3)
            throw new Exception("El nombre debe contener al menos 3 caracteres.");

        if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new Exception("El email no es válido.");

        if (dto.Password.Length < 6)
            throw new Exception("La contraseña debe tener al menos 6 caracteres.");

        var newUser = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        };

        await _userService.CreateUserAsync(newUser);

        var token = _jwtService.GenerateToken(newUser);

        return new AuthResponseDto
        {
            UserId = newUser.Id,
            Email = newUser.Email,
            Token = token,
        };
    }


    // ================
    //  LOGIN
    // ================
    public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
    {
        // Se encuentra al usuario
        var user = await _userService.GetByEmailAsync(dto.Email);
        if (user == null)
            throw new Exception("Email/Contraseña incorrecta.");

        // Se verifica si las contraseñas coinciden.
        bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!isValid)
            throw new Exception("Email/Contraseña incorrecta.");

        // Se genera el token.
        var token = _jwtService.GenerateToken(user);

        // Devuelve la respuesta Auth.
        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Token = token,
        };
    }
}