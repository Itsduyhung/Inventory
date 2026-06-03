using InventoryDashboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<User> Users => Set<User>();
    public DbSet<SaleRecord> SaleRecords => Set<SaleRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
