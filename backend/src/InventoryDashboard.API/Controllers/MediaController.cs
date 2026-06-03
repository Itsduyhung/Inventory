using InventoryDashboard.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class MediaController(IImageStorageService imageStorage) : ControllerBase
{
    private static readonly string[] AllowedTypes =
        ["image/jpeg", "image/png", "image/webp", "image/gif"];

    [HttpPost("upload-image")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<ImageUploadResponse>> UploadImage(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest(new { message = "No file uploaded." });

        if (file.Length > 10 * 1024 * 1024)
            return BadRequest(new { message = "Image must be 10 MB or smaller." });

        if (!AllowedTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { message = "Only JPEG, PNG, WebP, and GIF images are allowed." });

        try
        {
            await using var stream = file.OpenReadStream();
            var url = await imageStorage.UploadVehicleImageAsync(
                stream,
                file.FileName,
                cancellationToken);
            return Ok(new ImageUploadResponse(url));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record ImageUploadResponse(string Url);
