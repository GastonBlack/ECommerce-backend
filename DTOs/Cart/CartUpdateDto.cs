using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.Cart;

public class CartUpdateDto
{
    [Required, Range(1, 999)]
    public int Quantity { get; set; }
}
