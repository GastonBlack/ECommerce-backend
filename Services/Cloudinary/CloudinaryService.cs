using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerceAPI.Settings;
using Microsoft.Extensions.Options;

namespace ECommerceAPI.Services.ImageUpload;

public class CloudinaryService : ICloudinaryService
{
    // ===============================================================
    private readonly Cloudinary _cloudinary;
    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }
    // ===============================================================

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file.Length == 0)
            throw new Exception("Archivo vacío");

        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "products", // carpeta dentro de Cloudinary
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.StatusCode != System.Net.HttpStatusCode.OK)
            throw new Exception("Error al subir imagen");

        return result.Url.ToString();
    }
}
