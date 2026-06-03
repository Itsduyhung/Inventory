using InventoryDashboard.Domain.Entities;
using InventoryDashboard.Domain.Interfaces;
using InventoryDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Infrastructure.Repositories;

public class SaleRepository(ApplicationDbContext context) : ISaleRepository
{
    public async Task<SaleRecord> AddAsync(SaleRecord sale, CancellationToken cancellationToken = default)
    {
        context.SaleRecords.Add(sale);
        await context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<(IReadOnlyList<SaleRecord> Items, int TotalCount)> GetByStaffPagedAsync(
        Guid staffUserId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.SaleRecords
            .Include(s => s.Vehicle)
            .Include(s => s.StaffUser)
            .Where(s => s.StaffUserId == staffUserId);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.SoldAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<SaleRecord?> GetByIdWithDetailsAsync(
        Guid id,
        Guid staffUserId,
        CancellationToken cancellationToken = default)
    {
        return context.SaleRecords
            .Include(s => s.Vehicle)
            .Include(s => s.StaffUser)
            .FirstOrDefaultAsync(s => s.Id == id && s.StaffUserId == staffUserId, cancellationToken);
    }

    public async Task<(IReadOnlyList<SaleRecord> Items, int TotalCount)> GetAllPagedAsync(
        int page,
        int pageSize,
        Guid? staffUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.SaleRecords
            .Include(s => s.Vehicle)
            .Include(s => s.StaffUser)
            .AsQueryable();

        if (staffUserId is not null)
            query = query.Where(s => s.StaffUserId == staffUserId);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.SoldAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<SaleRecord?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return context.SaleRecords
            .Include(s => s.Vehicle)
            .Include(s => s.StaffUser)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}
