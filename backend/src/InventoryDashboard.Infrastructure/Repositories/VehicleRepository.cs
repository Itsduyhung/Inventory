using InventoryDashboard.Domain.Entities;
using InventoryDashboard.Domain.Interfaces;
using InventoryDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Infrastructure.Repositories;

public class VehicleRepository(ApplicationDbContext context) : IVehicleRepository
{
    public async Task<(IReadOnlyList<Vehicle> Items, int TotalCount)> GetPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilters(context.Vehicles.AsQueryable(), search, make, model, minAgeDays, maxAgeDays, agingOnly);

        if (staffAvailableOnly)
            query = query.Where(v => !v.IsDisabled && v.Quantity > 0);
        else if (!includeDisabled)
            query = query.Where(v => !v.IsDisabled);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(v => v.DateAddedToInventory)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Vehicles.FindAsync([id], cancellationToken).AsTask();
    }

    public async Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync(cancellationToken);
        return vehicle;
    }

    public async Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        context.Vehicles.Update(vehicle);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(int TotalUnits, int AgingCount, int WithNoteCount)> GetSummaryCountsAsync(
        CancellationToken cancellationToken = default)
    {
        var vehicles = await context.Vehicles.ToListAsync(cancellationToken);
        var agingCutoff = DateTime.UtcNow.Date.AddDays(-90);

        var totalUnits = vehicles.Where(v => !v.IsDisabled).Sum(v => v.Quantity);
        var agingCount = vehicles.Count(v => v.DateAddedToInventory.Date < agingCutoff && !v.IsDisabled);
        var withNote = vehicles.Count(v => !string.IsNullOrWhiteSpace(v.Note));

        return (totalUnits, agingCount, withNote);
    }

    private static IQueryable<Vehicle> ApplyFilters(
        IQueryable<Vehicle> query,
        string? search,
        string? make,
        string? model,
        int? minAgeDays,
        int? maxAgeDays,
        bool? agingOnly)
    {
        if (!string.IsNullOrWhiteSpace(make))
        {
            var makeTerm = make.Trim().ToLower();
            query = query.Where(v => v.Make.ToLower().Contains(makeTerm));
        }

        if (!string.IsNullOrWhiteSpace(model))
            query = query.Where(v => v.Model.ToLower().Contains(model.ToLower()));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(v =>
                v.Make.ToLower().Contains(term) ||
                v.Model.ToLower().Contains(term) ||
                v.Vin.ToLower().Contains(term) ||
                (v.Note != null && v.Note.ToLower().Contains(term)));
        }

        if (minAgeDays.HasValue)
        {
            var maxDate = DateTime.UtcNow.Date.AddDays(-minAgeDays.Value);
            query = query.Where(v => v.DateAddedToInventory <= maxDate);
        }

        if (maxAgeDays.HasValue)
        {
            var minDate = DateTime.UtcNow.Date.AddDays(-maxAgeDays.Value);
            query = query.Where(v => v.DateAddedToInventory >= minDate);
        }

        if (agingOnly == true)
        {
            var agingCutoff = DateTime.UtcNow.Date.AddDays(-90);
            query = query.Where(v => v.DateAddedToInventory < agingCutoff);
        }

        return query;
    }
}
