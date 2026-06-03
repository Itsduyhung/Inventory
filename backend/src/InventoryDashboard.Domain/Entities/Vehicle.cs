namespace InventoryDashboard.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Vin { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsDisabled { get; set; }
    public string? Note { get; set; }
    public DateTime DateAddedToInventory { get; set; }

    public string Color { get; set; } = string.Empty;
    public string ExteriorColor { get; set; } = string.Empty;
    public string InteriorColor { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public string FuelType { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string Engine { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<SaleRecord> SaleRecords { get; set; } = [];

    public int DaysInInventory => (DateTime.UtcNow.Date - DateAddedToInventory.Date).Days;
    public bool IsAgingStock => DaysInInventory > 90;
    public bool IsLowStock => !IsDisabled && Quantity == 1;
    public bool IsAvailableForStaff => !IsDisabled && Quantity > 0;
}
