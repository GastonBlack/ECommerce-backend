namespace ECommerceAPI.Models;

public class Order
{
    public int Id { get; set; }

    // Relación con el usuario.
    public int UserId { get; set; }
    public User User { get; set; }

    // Fecha de creación de la orden.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Total de la orden calculado.
    public decimal TotalAmount { get; set; }

    // Lista de items dentro de la orden.
    public List<OrderItem> Items { get; set; } = [];
}