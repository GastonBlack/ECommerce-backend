namespace ECommerceAPI.DTOs.Auth;

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;  // Para uso posterior en React.
}