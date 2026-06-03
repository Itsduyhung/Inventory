using InventoryDashboard.Domain.Entities;

namespace InventoryDashboard.Domain.Interfaces;

public interface ISaleRepository
{
    Task<SaleRecord> AddAsync(SaleRecord sale, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<SaleRecord> Items, int TotalCount)> GetByStaffPagedAsync(
        Guid staffUserId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<SaleRecord?> GetByIdWithDetailsAsync(Guid id, Guid staffUserId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<SaleRecord> Items, int TotalCount)> GetAllPagedAsync(
        int page,
        int pageSize,
        Guid? staffUserId = null,
        CancellationToken cancellationToken = default);

    Task<SaleRecord?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
