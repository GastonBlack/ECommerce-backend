namespace ECommerceAPI.DTOs.Auth;

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;  // Para uso posterior en React.
}