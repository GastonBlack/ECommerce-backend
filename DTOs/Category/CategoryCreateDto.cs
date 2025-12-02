using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.Category;

public class CategoryCreateDto
{
    [Required, MinLength(2), MaxLength(40)]
    public string Name { get; set; }
}