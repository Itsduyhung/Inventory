using InventoryDashboard.Application.DTOs;

namespace InventoryDashboard.Application.Interfaces;

public interface IVehicleService
{
    Task<PagedResultDto<VehicleDto>> GetVehiclesAsync(
        VehicleQueryDto query,
        bool includeDisabled,
        bool staffAvailableOnly,
        CancellationToken cancellationToken = default);

    Task<InventorySummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<VehicleDetailDto?> GetVehicleDetailAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VehicleDetailDto> CreateVehicleAsync(CreateVehicleDto dto, CancellationToken cancellationToken = default);
    Task<VehicleDetailDto?> UpdateVehicleAsync(Guid id, UpdateVehicleDto dto, CancellationToken cancellationToken = default);
    Task<VehicleDetailDto?> DisableVehicleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VehicleDetailDto?> AddQuantityAsync(Guid id, int amount, CancellationToken cancellationToken = default);
}
