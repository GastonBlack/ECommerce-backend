namespace ECommerceAPI.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }

    public int Stock { get; set; }
    public int TotalSold { get; set; } = 0;

    public string ImageUrl { get; set; } = null!;       // Cada producto tendrá su propia imagen.

    // Relación con Categoría.
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}