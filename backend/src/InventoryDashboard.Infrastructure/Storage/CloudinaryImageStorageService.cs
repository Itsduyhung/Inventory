using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InventoryDashboard.Infrastructure.Storage;

public class CloudinaryImageStorageService(
    IOptions<CloudinarySettings> options,
    ILogger<CloudinaryImageStorageService> logger) : IImageStorageService
{
    private readonly CloudinarySettings _settings = options.Value;

    public async Task<string> UploadVehicleImageAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.CloudName)
            || string.IsNullOrWhiteSpace(_settings.ApiKey)
            || string.IsNullOrWhiteSpace(_settings.ApiSecret))
        {
            throw new InvalidOperationException("Cloudinary is not configured.");
        }

        var account = new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret);
        var cloudinary = new Cloudinary(account);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream),
            Folder = _settings.Folder,
            Overwrite = false,
            UniqueFilename = true,
            UseFilename = true
        };

        var result = await cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.Error is not null)
        {
            logger.LogError("Cloudinary upload failed: {Message}", result.Error.Message);
            throw new InvalidOperationException(result.Error.Message ?? "Image upload failed.");
        }

        return result.SecureUrl?.ToString()
            ?? throw new InvalidOperationException("Cloudinary did not return an image URL.");
    }
}
