using BCrypt.Net;
using ECommerceAPI.Data;
using ECommerceAPI.DTOs.Auth;
using ECommerceAPI.Models;
using ECommerceAPI.Services.Jwt;
using ECommerceAPI.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services.Auth;

public class AuthService : IAuthService
{
    // =================================================================
    private readonly AppDbContext _db;
    private readonly IJwtService _jwtService;

    public AuthService(AppDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }
    // =================================================================

    // ================
    //  REGISTER
    // ================
    public async Task<AuthResponseDto> RegisterAsync(UserRegisterDto dto)
    {
        // Verificar email.
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingUser != null)
            throw new Exception("El email ya está registrado.");

        // Crear el usuario
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password) // Se guarda el hash.
        };

        // Guarda al usuario en la db.
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Genera el token JWT.
        var token = _jwtService.GenerateToken(user);

        // Devuelve la respuesta
        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Token = token,
        };
    }
    

    // ================
    //  LOGIN
    // ================
    public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
    {
        // Busca al usuario por email.
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            throw new Exception("Credenciales invalidas.");

        // Comparar contraseña con el hash.
        bool isValidPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!isValidPassword)
            throw new Exception("Credenciales invalidas.");

        // Generar token
        var token = _jwtService.GenerateToken(user);

        // Devolver DTO
        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Token = token,
        };
    }
}