namespace ECommerceAPI.DTOs.Payment;

public class CreatePreferenceResponseDto
{
    public int OrderId { get; set; }
    public string? InitPoint { get; set; }
    public string? SandboxInitPoint { get; set; }
}