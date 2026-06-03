using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryDashboard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<ISaleService, SaleService>();
        return services;
    }
}
