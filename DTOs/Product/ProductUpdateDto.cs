using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.Product;

public class ProductUpdateDto
{
    [Required, MinLength(2), MaxLength(80)]
    public string Name { get; set; }

    [Required, MinLength(2), MaxLength(400)]
    public string Description { get; set; }

    [Required, Range(0.1, 100000)]
    public decimal Price { get; set; }

    [Required, Range(0, 99999)]
    public int Stock { get; set; }

    [Required, Url]
    public string ImageUrl { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}