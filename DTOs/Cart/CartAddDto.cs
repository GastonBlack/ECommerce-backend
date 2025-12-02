using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.Cart;

public class CartAddDto
{
    [Required]
    public int ProductId { get; set; }

    [Required, Range(1, 999)]
    public int Quantity { get; set; }
}
