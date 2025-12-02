using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.Auth;

public class UserLoginDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(6), MaxLength(32)]
    public string Password { get; set; } = null!;
}