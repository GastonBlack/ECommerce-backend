using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.Auth;

// Falta agregar los campops de celular y dirección.
public class UserRegisterDto
{
    [Required, MinLength(2), MaxLength(60)]
    public string FullName { get; set; } = null!;

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(6), MaxLength(32)]
    public string Password { get; set; } = null!;
}