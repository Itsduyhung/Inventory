namespace InventoryDashboard.Application.Interfaces;

public interface IImageStorageService
{
    Task<string> UploadVehicleImageAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);
}
