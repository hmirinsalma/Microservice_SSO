using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Domain.Entities;
using ONEE.SSO.Infrastructure.Security;

namespace ONEE.SSO.Infrastructure.Persistence.Seed;

public static class ClientApplicationsSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedGestionPersonnelAsync(context);
        await SeedTimsAsync(context);
        await SeedEamsAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedGestionPersonnelAsync(ApplicationDbContext context)
    {
        const string clientId = "gestion-personnel";

        if (await context.ClientApplications.AnyAsync(c => c.ClientId == clientId))
            return;

        var passwordHasher = new BCryptPasswordHasher();
        var clientSecret = "gestion-personnel-secret-2024"; // Secret pour dev
        var hashedSecret = passwordHasher.Hash(clientSecret);

        context.ClientApplications.Add(new ClientApplication
        {
            Id = Guid.NewGuid(),
            Name = "Gestion du Personnel",
            ClientId = clientId,
            ClientSecret = hashedSecret,
            RedirectUri = "http://localhost:5173/callback",
            PostLogoutRedirectUri = "http://localhost:5173/login",
            
            // Scopes selon la fiche de l'application
            AllowedScopes = "openid profile email roles offline_access gestion-personnel",
            
            AllowedGrantTypes = "authorization_code refresh_token",
            
            RequirePkce = true,
            RequireConsent = false,
            
            // 15 minutes access token
            AccessTokenLifetime = 900,
            
            // 30 jours refresh token
            RefreshTokenLifetime = 2592000,
            
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
    }

    private static async Task SeedTimsAsync(ApplicationDbContext context)
    {
        const string clientId = "tims-app";

        if (await context.ClientApplications.AnyAsync(c => c.ClientId == clientId))
            return;

        var passwordHasher = new BCryptPasswordHasher();
        var clientSecret = "tims-app-secret-2024"; // Secret pour dev
        var hashedSecret = passwordHasher.Hash(clientSecret);

        context.ClientApplications.Add(new ClientApplication
        {
            Id = Guid.NewGuid(),
            Name = "ONEE TIMS",
            ClientId = clientId,
            ClientSecret = hashedSecret,
            RedirectUri = "http://localhost:5173/callback",
            PostLogoutRedirectUri = "http://localhost:5173/login",
            
            // Scopes custom pour TIMS selon la fiche
            AllowedScopes = "openid profile email roles offline_access tims_user_id tims_service_id tims_team_id tims_roles",
            
            AllowedGrantTypes = "authorization_code refresh_token",
            
            RequirePkce = true,
            RequireConsent = false,
            
            // 60 minutes access token (1 heure)
            AccessTokenLifetime = 3600,
            
            // 24 heures refresh token
            RefreshTokenLifetime = 86400,
            
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
    }

    private static async Task SeedEamsAsync(ApplicationDbContext context)
    {
        const string clientId = "eams-spa";

        if (await context.ClientApplications.AnyAsync(c => c.ClientId == clientId))
            return;

        var passwordHasher = new BCryptPasswordHasher();
        var clientSecret = "eams-spa-secret-2024"; // Secret pour dev
        var hashedSecret = passwordHasher.Hash(clientSecret);

        context.ClientApplications.Add(new ClientApplication
        {
            Id = Guid.NewGuid(),
            Name = "ONEE EAMS",
            ClientId = clientId,
            ClientSecret = hashedSecret,
            RedirectUri = "http://localhost:5173/auth/callback",
            PostLogoutRedirectUri = "http://localhost:5173/login",
            
            // Scopes custom pour EAMS selon la fiche
            AllowedScopes = "openid profile email roles offline_access eams eams_user_id serviceId",
            
            AllowedGrantTypes = "authorization_code refresh_token",
            
            RequirePkce = true,
            RequireConsent = false,
            
            // 30 minutes access token
            AccessTokenLifetime = 1800,
            
            // 30 jours refresh token
            RefreshTokenLifetime = 2592000,
            
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
    }
}