using Microsoft.Extensions.DependencyInjection;
using ONEE.SSO.Application.Features.Auth.Handlers;
using ONEE.SSO.Application.Features.Users.Handlers;

namespace ONEE.SSO.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Auth Handlers
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<LogoutCommandHandler>();
        services.AddScoped<ValidateTokenCommandHandler>();
        services.AddScoped<RefreshTokenCommandHandler>();
        services.AddScoped<ForgotPasswordCommandHandler>();
        services.AddScoped<ResetPasswordCommandHandler>();
        services.AddScoped<ChangePasswordCommandHandler>();
        
        // User Handlers
        services.AddScoped<UnlockUserCommandHandler>();

        return services;
    }
}