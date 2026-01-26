using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.User;

public class UserChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; } = null!;

    [Required, MinLength(6), MaxLength(32)]
    public string NewPassword { get; set; } = null!;
}