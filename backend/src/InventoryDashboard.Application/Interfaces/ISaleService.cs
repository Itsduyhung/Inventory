using InventoryDashboard.Application.DTOs;

namespace InventoryDashboard.Application.Interfaces;

public interface ISaleService
{
    Task<VehicleDto> ProcessSaleAsync(Guid vehicleId, Guid staffUserId, CancellationToken cancellationToken = default);

    Task<PagedResultDto<SaleRecordDto>> GetStaffHistoryAsync(
        Guid staffUserId,
        SaleHistoryQueryDto query,
        CancellationToken cancellationToken = default);

    Task<SaleRecordDetailDto?> GetStaffSaleDetailAsync(
        Guid saleId,
        Guid staffUserId,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<SaleRecordDto>> GetAllSalesHistoryAsync(
        AdminSaleHistoryQueryDto query,
        CancellationToken cancellationToken = default);

    Task<SaleRecordDetailDto?> GetAdminSaleDetailAsync(
        Guid saleId,
        CancellationToken cancellationToken = default);
}
