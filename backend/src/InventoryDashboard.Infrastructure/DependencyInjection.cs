using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Domain.Interfaces;
using InventoryDashboard.Infrastructure.Auth;
using InventoryDashboard.Infrastructure.Email;
using InventoryDashboard.Infrastructure.Options;
using InventoryDashboard.Infrastructure.Persistence;
using InventoryDashboard.Infrastructure.Repositories;
using InventoryDashboard.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryDashboard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();

        return services;
    }
}
