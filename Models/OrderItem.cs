namespace ECommerceAPI.Models;

public class OrderItem
{
    public int Id { get; set; }

    // Relación con la orden.
    public int OrderId { get; set; }
    public Order Order { get; set; }

    // Relacion con el producto.
    public int ProductId { get; set; }
    public Product Product { get; set; }

    // Datos al momento de comprar.
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }
}