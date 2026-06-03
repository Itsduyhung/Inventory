using InventoryDashboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryDashboard.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Username).IsUnique();
        builder.Property(u => u.Username).HasMaxLength(50).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(200).IsRequired();
        builder.Property(u => u.FullName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(120);
        builder.HasIndex(u => u.Email).IsUnique().HasFilter("\"Email\" IS NOT NULL");
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        builder.Property(u => u.RefreshTokenHash).HasMaxLength(128);
        builder.Property(u => u.PasswordResetToken).HasMaxLength(128);
    }
}
