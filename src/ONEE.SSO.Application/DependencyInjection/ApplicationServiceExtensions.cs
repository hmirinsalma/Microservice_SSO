using Microsoft.Extensions.DependencyInjection;

namespace ONEE.SSO.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Les services Application seront enregistrés ici.

        return services;
    }
}