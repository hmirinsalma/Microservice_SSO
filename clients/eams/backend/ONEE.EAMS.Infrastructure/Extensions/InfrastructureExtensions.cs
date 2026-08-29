using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Infrastructure.BackgroundServices;
using ONEE.EAMS.Infrastructure.Data;
using ONEE.EAMS.Infrastructure.Seed;
using ONEE.EAMS.Infrastructure.Services;

namespace ONEE.EAMS.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAppDbContext, AppDbContextAdapter>();
        // Phase STUB (dev/démo) — remplacer par SsoAuthService lors de l'intégration SSO
        services.AddScoped<IAuthService, StubAuthService>();
        // TODO SSO : décommenter lors de l'intégration
        // services.AddScoped<IAuthService, SsoAuthService>();
        // services.AddHttpClient<ISsoService, SsoService>(client =>
        // {
        //     client.BaseAddress = new Uri(config["Sso:BaseUrl"]!);
        // });
        services.AddScoped<IReferenceGeneratorService, ReferenceGeneratorService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<DataSeeder>();
        services.AddHostedService<MaintenanceStatusUpdater>();

        return services;
    }
}
