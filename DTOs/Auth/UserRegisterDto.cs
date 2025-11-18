namespace ECommerceAPI.DTOs.Auth;

// Falta agregar los campops de celular y dirección.
public class UserRegisterDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}