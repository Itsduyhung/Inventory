using InventoryDashboard.Domain.Enums;

namespace InventoryDashboard.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? RefreshTokenHash { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetExpiresAt { get; set; }

    public ICollection<SaleRecord> SaleRecords { get; set; } = [];
}
