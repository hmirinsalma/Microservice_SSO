using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ONEE.SSO.Shared.Settings;

namespace ONEE.SSO.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ONEE SSO API",
                Version = "v1",
                Description = "Microservice d'authentification SSO de ONEE"
            });
        });

        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        services.Configure<DatabaseSettings>(
            configuration.GetSection(DatabaseSettings.SectionName));

        services.Configure<CorsSettings>(
            configuration.GetSection(CorsSettings.SectionName));

        return services;
    }
}