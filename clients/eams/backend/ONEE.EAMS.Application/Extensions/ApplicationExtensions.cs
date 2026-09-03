using Microsoft.Extensions.DependencyInjection;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Application.Services;

namespace ONEE.EAMS.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategorieService, CategorieService>();
        services.AddScoped<IEquipementService, EquipementService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IUserService, UserService>();
        
        // 🎯 Service de provisioning automatique SSO
        services.AddScoped<SsoProvisioningService>();
        
        return services;
    }
}
