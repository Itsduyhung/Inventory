using InventoryDashboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryDashboard.Infrastructure.Persistence.Configurations;

public class SaleRecordConfiguration : IEntityTypeConfiguration<SaleRecord>
{
    public void Configure(EntityTypeBuilder<SaleRecord> builder)
    {
        builder.ToTable("sale_records");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SalePrice).HasPrecision(18, 2);
        builder.Property(s => s.Note).HasMaxLength(500);

        builder.HasOne(s => s.Vehicle)
            .WithMany(v => v.SaleRecords)
            .HasForeignKey(s => s.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.StaffUser)
            .WithMany(u => u.SaleRecords)
            .HasForeignKey(s => s.StaffUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
