namespace ECommerceAPI.DTOs.User;

public class UserMeResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string Rol { get; set; } = null!;
}