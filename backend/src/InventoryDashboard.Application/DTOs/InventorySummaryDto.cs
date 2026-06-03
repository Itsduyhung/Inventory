namespace InventoryDashboard.Application.DTOs;

public record InventorySummaryDto(
    int TotalVehicles,
    int AgingStockCount,
    int VehiclesWithNote);
