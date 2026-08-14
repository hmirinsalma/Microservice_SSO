using Microsoft.Extensions.DependencyInjection;
using ONEE.SSO.Application.Features.Auth.Handlers;

namespace ONEE.SSO.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<LogoutCommandHandler>();
        services.AddScoped<ValidateTokenCommandHandler>();
        services.AddScoped<RefreshTokenCommandHandler>();

        return services;
    }
}