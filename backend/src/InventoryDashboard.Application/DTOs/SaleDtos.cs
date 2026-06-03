namespace InventoryDashboard.Application.DTOs;

public record SaleRecordDto(
    Guid Id,
    Guid VehicleId,
    string VehicleTitle,
    string Vin,
    int QuantitySold,
    decimal SalePrice,
    int RemainingQuantityAfterSale,
    DateTime SoldAt,
    string? Note,
    string StaffName,
    Guid StaffUserId,
    string StaffUsername,
    string? VehicleImageUrl = null);

public record SaleRecordDetailDto(
    Guid Id,
    int QuantitySold,
    decimal SalePrice,
    int RemainingQuantityAfterSale,
    DateTime SoldAt,
    string? Note,
    string StaffName,
    VehicleDetailDto Vehicle);

public record SaleHistoryQueryDto(int Page = 1, int PageSize = 10);

public record AdminSaleHistoryQueryDto(
    int Page = 1,
    int PageSize = 10,
    Guid? StaffUserId = null);
