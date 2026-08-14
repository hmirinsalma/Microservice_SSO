using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Persistence.Seed;

public static class ClientApplicationsSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedRhAsync(context);
        await SeedTimsAsync(context);
        await SeedEamsAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedRhAsync(ApplicationDbContext context)
    {
        const string clientId = "rh-client";

        if (await context.ClientApplications.AnyAsync(c => c.ClientId == clientId))
            return;

        context.ClientApplications.Add(new ClientApplication
        {
            Id = Guid.NewGuid(),
            Name = "Gestion RH",
            ClientId = clientId,
            ClientSecret = Guid.NewGuid().ToString("N"),
            RedirectUri = "http://localhost:5173/callback",
            PostLogoutRedirectUri = "http://localhost:5173/login",

            AllowedScopes = "openid profile email roles offline_access rh",

            AllowedGrantTypes = "authorization_code",

            RequirePkce = true,

            RequireConsent = false,

            AccessTokenLifetime = 1800,

            RefreshTokenLifetime = 28800,

            IsActive = true,

            CreatedAt = DateTime.UtcNow
        });
    }

    private static async Task SeedTimsAsync(ApplicationDbContext context)
    {
        const string clientId = "tims-client";

        if (await context.ClientApplications.AnyAsync(c => c.ClientId == clientId))
            return;

        context.ClientApplications.Add(new ClientApplication
        {
            Id = Guid.NewGuid(),
            Name = "TIMS",
            ClientId = clientId,
            ClientSecret = Guid.NewGuid().ToString("N"),
            RedirectUri = "http://localhost:5173/callback",
            PostLogoutRedirectUri = "http://localhost:5173/login",

            AllowedScopes =
                "openid profile email roles tims_user_id serviceId teamId offline_access",

            AllowedGrantTypes = "authorization_code",

            RequirePkce = true,

            RequireConsent = false,

            AccessTokenLifetime = 3600,

            RefreshTokenLifetime = 28800,

            IsActive = true,

            CreatedAt = DateTime.UtcNow
        });
    }

    private static async Task SeedEamsAsync(ApplicationDbContext context)
    {
        const string clientId = "eams-client";

        if (await context.ClientApplications.AnyAsync(c => c.ClientId == clientId))
            return;

        context.ClientApplications.Add(new ClientApplication
        {
            Id = Guid.NewGuid(),
            Name = "EAMS",
            ClientId = clientId,
            ClientSecret = Guid.NewGuid().ToString("N"),
            RedirectUri = "http://localhost:5173/auth/callback",
            PostLogoutRedirectUri = "http://localhost:5173/login",

            AllowedScopes =
                "openid profile email roles offline_access eams",

            AllowedGrantTypes = "authorization_code",

            RequirePkce = true,

            RequireConsent = false,

            AccessTokenLifetime = 1800,

            RefreshTokenLifetime = 28800,

            IsActive = true,

            CreatedAt = DateTime.UtcNow
        });
    }
}