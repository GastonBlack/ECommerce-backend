using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.User;

public class UserUpdateMeDto
{
    [Required, MinLength(2), MaxLength(60)]
    public string FullName { get; set; } = null!;

    [MaxLength(150)]
    public string? Address { get; set; }

    [Phone]
    [MaxLength(30)]
    public string? Phone { get; set; }
}