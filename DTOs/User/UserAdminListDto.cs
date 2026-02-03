namespace ECommerceAPI.DTOs.User;

public class UserAdminListDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Rol { get; set; } = "User";
    public bool IsDisabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DisabledAt { get; set; }
}
