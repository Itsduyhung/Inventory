using InventoryDashboard.Application.DTOs;
using InventoryDashboard.Domain.Entities;

namespace InventoryDashboard.Application.Mappings;

public static class VehicleMapper
{
    public static VehicleDto ToDto(Vehicle v) => new(
        v.Id, v.Make, v.Model, v.Year, v.Vin, v.Price, v.Quantity, v.IsDisabled, v.Note,
        v.DateAddedToInventory, v.DaysInInventory, v.IsAgingStock, v.IsLowStock,
        v.Color, v.ExteriorColor, v.InteriorColor, v.Mileage, v.FuelType, v.Transmission,
        v.Engine, v.BodyType, v.Description, v.ImageUrl);

    public static VehicleDetailDto ToDetailDto(Vehicle v) => new(
        v.Id, v.Make, v.Model, v.Year, v.Vin, v.Price, v.Quantity, v.IsDisabled, v.Note,
        v.DateAddedToInventory, v.DaysInInventory, v.IsAgingStock, v.IsLowStock,
        v.Color, v.ExteriorColor, v.InteriorColor, v.Mileage, v.FuelType, v.Transmission,
        v.Engine, v.BodyType, v.Description, v.ImageUrl, v.CreatedAt, v.UpdatedAt);

    public static Vehicle FromCreateDto(CreateVehicleDto dto) => new()
    {
        Id = Guid.NewGuid(),
        Make = dto.Make.Trim(),
        Model = dto.Model.Trim(),
        Year = dto.Year,
        Vin = dto.Vin.Trim().ToUpperInvariant(),
        Price = dto.Price,
        Quantity = dto.Quantity,
        Note = dto.Note,
        DateAddedToInventory = DateTimeHelper.ToUtcDate(
            dto.DateAddedToInventory ?? DateTime.UtcNow),
        Color = dto.Color,
        ExteriorColor = dto.ExteriorColor,
        InteriorColor = dto.InteriorColor,
        Mileage = dto.Mileage,
        FuelType = dto.FuelType,
        Transmission = dto.Transmission,
        Engine = dto.Engine,
        BodyType = dto.BodyType,
        Description = dto.Description,
        ImageUrl = dto.ImageUrl,
        CreatedAt = DateTimeHelper.ToUtc(DateTime.UtcNow)
    };

    public static void ApplyUpdate(Vehicle vehicle, UpdateVehicleDto dto)
    {
        vehicle.Make = dto.Make.Trim();
        vehicle.Model = dto.Model.Trim();
        vehicle.Year = dto.Year;
        vehicle.Vin = dto.Vin.Trim().ToUpperInvariant();
        vehicle.Price = dto.Price;
        vehicle.Quantity = dto.Quantity;
        vehicle.Note = dto.Note;
        vehicle.DateAddedToInventory = DateTimeHelper.ToUtcDate(dto.DateAddedToInventory);
        vehicle.Color = dto.Color;
        vehicle.ExteriorColor = dto.ExteriorColor;
        vehicle.InteriorColor = dto.InteriorColor;
        vehicle.Mileage = dto.Mileage;
        vehicle.FuelType = dto.FuelType;
        vehicle.Transmission = dto.Transmission;
        vehicle.Engine = dto.Engine;
        vehicle.BodyType = dto.BodyType;
        vehicle.Description = dto.Description;
        vehicle.ImageUrl = dto.ImageUrl;
        vehicle.UpdatedAt = DateTimeHelper.ToUtc(DateTime.UtcNow);

        if (vehicle.Quantity <= 0)
        {
            vehicle.Quantity = 0;
            vehicle.IsDisabled = true;
        }
    }
}
