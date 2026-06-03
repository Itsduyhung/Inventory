using InventoryDashboard.Domain.Entities;
using InventoryDashboard.Domain.Enums;
using InventoryDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InventoryDashboard.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        await context.Database.MigrateAsync();

        await SeedUsersAsync(context, logger);
        await SeedVehiclesAsync(context, logger);
        await SeedSampleSalesAsync(context, logger);
    }

    private static async Task SeedUsersAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Users.AnyAsync())
            return;

        var users = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FullName = "System Administrator",
                Role = UserRole.Admin
            },
            new()
            {
                Id = Guid.NewGuid(),
                Username = "staff",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("staff123"),
                FullName = "Sales Staff",
                Role = UserRole.Staff
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} users (admin/admin123, staff/staff123)", users.Count);
    }

    private static async Task SeedVehiclesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Vehicles.AnyAsync())
            return;

        var today = DateTime.UtcNow.Date;
        var vehicles = new List<Vehicle>
        {
            CreateVehicle("Toyota", "Camry", 2024, "1HGCM82633A004352", 28500, 3, today.AddDays(-45),
                "White", "Pearl White", "Black", 12500, "Hybrid", "Automatic", "2.5L 4-Cyl", "Sedan",
                "Well-maintained Camry with full service history. Popular family sedan."),
            CreateVehicle("Toyota", "RAV4", 2023, "2T1BURHE0JC123456", 32000, 2, today.AddDays(-72),
                "Blue", "Blueprint", "Gray", 22000, "Gasoline", "Automatic", "2.5L 4-Cyl", "SUV",
                "Compact SUV with AWD. Great for all-weather driving."),
            CreateVehicle("Honda", "Civic", 2024, "19XFC2F59ME123789", 26500, 5, today.AddDays(-30),
                "Red", "Rallye Red", "Black", 8000, "Gasoline", "CVT", "2.0L 4-Cyl", "Sedan",
                "Sport trim Civic with low mileage. Single owner trade-in."),
            CreateVehicle("Honda", "Accord", 2023, "1HGCV1F34MA123456", 31000, 1, today.AddDays(-105),
                "Silver", "Lunar Silver", "Ivory", 35000, "Gasoline", "Automatic", "1.5L Turbo", "Sedan",
                "Aging stock — last unit. Note: consider price reduction.", note: "Price review needed"),
            CreateVehicle("Ford", "F-150", 2023, "1FTEW1EP5KFA12345", 45000, 2, today.AddDays(-120),
                "Black", "Agate Black", "Black", 41000, "Gasoline", "Automatic", "3.5L EcoBoost V6", "Truck",
                "XLT trim pickup. Tow package included."),
            CreateVehicle("Ford", "Mustang", 2024, "1FA6P8TH4R5123456", 42000, 4, today.AddDays(-60),
                "Yellow", "Race Red", "Black", 5000, "Gasoline", "Manual", "5.0L V8", "Coupe",
                "GT fastback. Enthusiast vehicle with premium audio."),
            CreateVehicle("BMW", "X5", 2022, "5UXCR6C05L9A12345", 52000, 1, today.AddDays(-150),
                "Gray", "Mineral Gray", "Cognac", 48000, "Gasoline", "Automatic", "3.0L I6 Turbo", "SUV",
                "Luxury SUV with premium package. Aging stock."),
            CreateVehicle("BMW", "3 Series", 2023, "WBA8E9G50NKA12345", 38000, 2, today.AddDays(-95),
                "White", "Alpine White", "Black", 28000, "Gasoline", "Automatic", "2.0L I4 Turbo", "Sedan",
                "330i sport line. Certified pre-owned eligible."),
            CreateVehicle("Mercedes-Benz", "C-Class", 2023, "W1KZF4FB3PA123456", 44000, 3, today.AddDays(-98),
                "Black", "Obsidian Black", "Beige", 32000, "Gasoline", "Automatic", "2.0L Turbo", "Sedan",
                "C300 with AMG line exterior. Panoramic roof."),
            CreateVehicle("Chevrolet", "Silverado", 2024, "1GCUYDED4RZ123456", 48000, 6, today.AddDays(-15),
                "White", "Summit White", "Jet Black", 3000, "Gasoline", "Automatic", "5.3L V8", "Truck",
                "LT Trail Boss. Brand new to lot."),
            CreateVehicle("Chevrolet", "Equinox", 2023, "3GNAXKEV6PS123456", 29000, 0, today.AddDays(-200),
                "Blue", "Bright Blue", "Gray", 55000, "Gasoline", "Automatic", "1.5L Turbo", "SUV",
                "Disabled sample — out of stock.", isDisabled: true),
            CreateVehicle("Nissan", "Altima", 2023, "1N4BL4BV8PN123456", 27000, 2, today.AddDays(-88),
                "Gray", "Gun Metallic", "Charcoal", 26000, "Gasoline", "CVT", "2.5L 4-Cyl", "Sedan",
                "SV trim with safety shield package.")
        };

        context.Vehicles.AddRange(vehicles);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} vehicles with detailed data", vehicles.Count);
    }

    private static Vehicle CreateVehicle(
        string make, string model, int year, string vin, decimal price, int quantity,
        DateTime dateAdded, string color, string exterior, string interior, int mileage,
        string fuel, string transmission, string engine, string body, string description,
        string? note = null, bool isDisabled = false) => new()
    {
        Id = Guid.NewGuid(),
        Make = make,
        Model = model,
        Year = year,
        Vin = vin,
        Price = price,
        Quantity = quantity,
        IsDisabled = isDisabled || quantity <= 0,
        Note = note,
        DateAddedToInventory = dateAdded,
        Color = color,
        ExteriorColor = exterior,
        InteriorColor = interior,
        Mileage = mileage,
        FuelType = fuel,
        Transmission = transmission,
        Engine = engine,
        BodyType = body,
        Description = description,
        CreatedAt = DateTime.UtcNow
    };

    private static async Task SeedSampleSalesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.SaleRecords.AnyAsync())
            return;

        var staff = await context.Users
            .FirstOrDefaultAsync(u => u.Username == "staff" && u.Role == UserRole.Staff);
        if (staff is null)
            return;

        var vehicles = await context.Vehicles
            .Where(v => !v.IsDisabled && v.Quantity > 0)
            .OrderBy(v => v.DateAddedToInventory)
            .Take(3)
            .ToListAsync();

        if (vehicles.Count == 0)
            return;

        var sales = new List<SaleRecord>();
        var dayOffset = 1;

        foreach (var vehicle in vehicles)
        {
            var remaining = vehicle.Quantity - 1;
            vehicle.Quantity = Math.Max(remaining, 0);
            if (vehicle.Quantity <= 0)
                vehicle.IsDisabled = true;
            vehicle.UpdatedAt = DateTime.UtcNow;

            sales.Add(new SaleRecord
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicle.Id,
                StaffUserId = staff.Id,
                QuantitySold = 1,
                SalePrice = vehicle.Price,
                RemainingQuantityAfterSale = remaining,
                SoldAt = DateTime.UtcNow.AddDays(-dayOffset),
                Note = "Sample sale (seed data)"
            });
            dayOffset += 2;
        }

        context.SaleRecords.AddRange(sales);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} sample sales for staff demo", sales.Count);
    }
}
