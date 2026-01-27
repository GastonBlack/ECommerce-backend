namespace ECommerceAPI.Models;

public class User
{
    public int Id { get; set; }
    
    public string FullName { get; set; } = null!; 
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public string Rol { get; set; } = "User";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Opcionales.
    public string? Address { get; set; }
    public string? Phone { get; set; }

    // Relaciones.
    public List<CartItem> CartItems { get; set; }   // Carrito.
    public List<Order> Orders { get; set; }         // Historial de compras.
}