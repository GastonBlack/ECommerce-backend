namespace ECommerceAPI.Services.ImageUpload
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
