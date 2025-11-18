namespace ECommerceAPI.Models;

public class CartItem
{
    public int Id { get; set; }

    // Relaciones.
    public int UserId { get; set; }
    public User User { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    // Campos del carrito.
    public int Quantity { get; set; }
}