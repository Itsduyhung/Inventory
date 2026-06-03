namespace InventoryDashboard.Domain.Entities;

public class SaleRecord
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public Guid StaffUserId { get; set; }
    public User StaffUser { get; set; } = null!;
    public int QuantitySold { get; set; } = 1;
    public decimal SalePrice { get; set; }
    public int RemainingQuantityAfterSale { get; set; }
    public DateTime SoldAt { get; set; } = DateTime.UtcNow;

    public string? Note { get; set; }
}
