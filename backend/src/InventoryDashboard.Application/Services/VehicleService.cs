using InventoryDashboard.Application.DTOs;
using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Application.Mappings;
using InventoryDashboard.Domain.Interfaces;

namespace InventoryDashboard.Application.Services;

public class VehicleService(IVehicleRepository repository) : IVehicleService
{
    public async Task<PagedResultDto<VehicleDto>> GetVehiclesAsync(
        VehicleQueryDto query,
        bool includeDisabled,
        bool staffAvailableOnly,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        var (items, total) = await repository.GetPagedAsync(
            page, pageSize, query.Search, query.Make, query.Model,
            query.MinAgeDays, query.MaxAgeDays, query.AgingOnly,
            includeDisabled, staffAvailableOnly, cancellationToken);

        var dtos = items.Select(VehicleMapper.ToDto).ToList();
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new PagedResultDto<VehicleDto>(dtos, page, pageSize, total, totalPages);
    }

    public async Task<InventorySummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var (totalUnits, agingCount, withNote) = await repository.GetSummaryCountsAsync(cancellationToken);
        return new InventorySummaryDto(totalUnits, agingCount, withNote);
    }

    public async Task<VehicleDetailDto?> GetVehicleDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await repository.GetByIdAsync(id, cancellationToken);
        return vehicle is null ? null : VehicleMapper.ToDetailDto(vehicle);
    }

    public async Task<VehicleDetailDto> CreateVehicleAsync(
        CreateVehicleDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Quantity < 0)
            throw new InvalidOperationException("Quantity cannot be negative.");

        var vehicle = VehicleMapper.FromCreateDto(dto);
        if (vehicle.Quantity <= 0)
        {
            vehicle.Quantity = 0;
            vehicle.IsDisabled = true;
        }

        try
        {
            await repository.AddAsync(vehicle, cancellationToken);
        }
        catch (Exception ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message;
            if (msg.Contains("IX_vehicles_Vin", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("A vehicle with this VIN already exists.");

            throw new InvalidOperationException("Could not save vehicle. Please check your data and try again.");
        }

        return VehicleMapper.ToDetailDto(vehicle);
    }

    public async Task<VehicleDetailDto?> UpdateVehicleAsync(
        Guid id,
        UpdateVehicleDto dto,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await repository.GetByIdAsync(id, cancellationToken);
        if (vehicle is null)
            return null;

        VehicleMapper.ApplyUpdate(vehicle, dto);
        await repository.UpdateAsync(vehicle, cancellationToken);
        return VehicleMapper.ToDetailDto(vehicle);
    }

    public async Task<VehicleDetailDto?> DisableVehicleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await repository.GetByIdAsync(id, cancellationToken);
        if (vehicle is null)
            return null;

        vehicle.Quantity = 0;
        vehicle.IsDisabled = true;
        vehicle.UpdatedAt = Mappings.DateTimeHelper.ToUtc(DateTime.UtcNow);
        await repository.UpdateAsync(vehicle, cancellationToken);
        return VehicleMapper.ToDetailDto(vehicle);
    }

    public async Task<VehicleDetailDto?> AddQuantityAsync(
        Guid id,
        int amount,
        CancellationToken cancellationToken = default)
    {
        if (amount < 1)
            throw new InvalidOperationException("Amount must be at least 1.");

        var vehicle = await repository.GetByIdAsync(id, cancellationToken);
        if (vehicle is null)
            return null;

        vehicle.Quantity += amount;
        vehicle.IsDisabled = false;
        vehicle.UpdatedAt = Mappings.DateTimeHelper.ToUtc(DateTime.UtcNow);
        await repository.UpdateAsync(vehicle, cancellationToken);
        return VehicleMapper.ToDetailDto(vehicle);
    }
}
