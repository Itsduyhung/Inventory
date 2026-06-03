using InventoryDashboard.Application.DTOs;
using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Application.Mappings;
using InventoryDashboard.Domain.Entities;
using InventoryDashboard.Domain.Interfaces;

namespace InventoryDashboard.Application.Services;

public class SaleService(
    IVehicleRepository vehicleRepository,
    ISaleRepository saleRepository,
    IUserRepository userRepository) : ISaleService
{
    public async Task<VehicleDto> ProcessSaleAsync(
        Guid vehicleId,
        Guid staffUserId,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(vehicleId, cancellationToken)
            ?? throw new InvalidOperationException("Vehicle not found.");

        if (vehicle.IsDisabled || vehicle.Quantity <= 0)
            throw new InvalidOperationException("This vehicle is not available for sale.");

        var staff = await userRepository.GetByIdAsync(staffUserId, cancellationToken)
            ?? throw new InvalidOperationException("Staff user not found.");

        vehicle.Quantity -= 1;
        var remaining = vehicle.Quantity;

        if (vehicle.Quantity <= 0)
        {
            vehicle.Quantity = 0;
            vehicle.IsDisabled = true;
        }

        vehicle.UpdatedAt = DateTimeHelper.ToUtc(DateTime.UtcNow);
        await vehicleRepository.UpdateAsync(vehicle, cancellationToken);

        var sale = new SaleRecord
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicle.Id,
            StaffUserId = staff.Id,
            QuantitySold = 1,
            SalePrice = vehicle.Price,
            RemainingQuantityAfterSale = remaining,
            SoldAt = DateTimeHelper.ToUtc(DateTime.UtcNow),
            Note = vehicle.IsDisabled ? "Auto-disabled: out of stock" : null
        };

        await saleRepository.AddAsync(sale, cancellationToken);
        return VehicleMapper.ToDto(vehicle);
    }

    public async Task<PagedResultDto<SaleRecordDto>> GetStaffHistoryAsync(
        Guid staffUserId,
        SaleHistoryQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        var (items, total) = await saleRepository.GetByStaffPagedAsync(
            staffUserId, page, pageSize, cancellationToken);

        var dtos = items.Select(ToSaleRecordDto).ToList();

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new PagedResultDto<SaleRecordDto>(dtos, page, pageSize, total, totalPages);
    }

    public async Task<SaleRecordDetailDto?> GetStaffSaleDetailAsync(
        Guid saleId,
        Guid staffUserId,
        CancellationToken cancellationToken = default)
    {
        var sale = await saleRepository.GetByIdWithDetailsAsync(saleId, staffUserId, cancellationToken);
        if (sale is null)
            return null;

        return new SaleRecordDetailDto(
            sale.Id,
            sale.QuantitySold,
            sale.SalePrice,
            sale.RemainingQuantityAfterSale,
            sale.SoldAt,
            sale.Note,
            sale.StaffUser.FullName,
            VehicleMapper.ToDetailDto(sale.Vehicle));
    }

    public async Task<PagedResultDto<SaleRecordDto>> GetAllSalesHistoryAsync(
        AdminSaleHistoryQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        var (items, total) = await saleRepository.GetAllPagedAsync(
            page, pageSize, query.StaffUserId, cancellationToken);

        var dtos = items.Select(ToSaleRecordDto).ToList();
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new PagedResultDto<SaleRecordDto>(dtos, page, pageSize, total, totalPages);
    }

    public async Task<SaleRecordDetailDto?> GetAdminSaleDetailAsync(
        Guid saleId,
        CancellationToken cancellationToken = default)
    {
        var sale = await saleRepository.GetByIdWithDetailsAsync(saleId, cancellationToken);
        if (sale is null)
            return null;

        return new SaleRecordDetailDto(
            sale.Id,
            sale.QuantitySold,
            sale.SalePrice,
            sale.RemainingQuantityAfterSale,
            sale.SoldAt,
            sale.Note,
            sale.StaffUser.FullName,
            VehicleMapper.ToDetailDto(sale.Vehicle));
    }

    private static SaleRecordDto ToSaleRecordDto(SaleRecord s) =>
        new(
            s.Id,
            s.VehicleId,
            $"{s.Vehicle.Year} {s.Vehicle.Make} {s.Vehicle.Model}",
            s.Vehicle.Vin,
            s.QuantitySold,
            s.SalePrice,
            s.RemainingQuantityAfterSale,
            s.SoldAt,
            s.Note,
            s.StaffUser.FullName,
            s.StaffUserId,
            s.StaffUser.Username,
            s.Vehicle.ImageUrl);
}
