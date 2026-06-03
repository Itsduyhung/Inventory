using InventoryDashboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryDashboard.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Make).HasMaxLength(100).IsRequired();
        builder.Property(v => v.Model).HasMaxLength(100).IsRequired();
        builder.Property(v => v.Vin).HasMaxLength(17).IsRequired();
        builder.HasIndex(v => v.Vin).IsUnique();

        builder.Property(v => v.Price).HasPrecision(18, 2);
        builder.Property(v => v.Note).HasMaxLength(1000);
        builder.Property(v => v.Color).HasMaxLength(50);
        builder.Property(v => v.ExteriorColor).HasMaxLength(50);
        builder.Property(v => v.InteriorColor).HasMaxLength(50);
        builder.Property(v => v.FuelType).HasMaxLength(50);
        builder.Property(v => v.Transmission).HasMaxLength(50);
        builder.Property(v => v.Engine).HasMaxLength(100);
        builder.Property(v => v.BodyType).HasMaxLength(50);
        builder.Property(v => v.Description).HasMaxLength(2000);
        builder.Property(v => v.ImageUrl).HasMaxLength(500);

        builder.Ignore(v => v.DaysInInventory);
        builder.Ignore(v => v.IsAgingStock);
        builder.Ignore(v => v.IsLowStock);
        builder.Ignore(v => v.IsAvailableForStaff);
    }
}
