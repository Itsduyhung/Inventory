using InventoryDashboard.Domain.Entities;

namespace InventoryDashboard.Domain.Interfaces;

public interface IVehicleRepository
{
    Task<(IReadOnlyList<Vehicle> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null,
        string? make = null,
        string? model = null,
        int? minAgeDays = null,
        int? maxAgeDays = null,
        bool? agingOnly = null,
        bool includeDisabled = false,
        bool staffAvailableOnly = false,
        CancellationToken cancellationToken = default);

    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<(int TotalUnits, int AgingCount, int WithNoteCount)> GetSummaryCountsAsync(
        CancellationToken cancellationToken = default);
}
