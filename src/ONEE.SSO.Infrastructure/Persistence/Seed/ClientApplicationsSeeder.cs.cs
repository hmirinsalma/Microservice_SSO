using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Domain.Entities;
using ONEE.SSO.Infrastructure.Security;

namespace ONEE.SSO.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeder pour les applications clientes OIDC
/// 
/// ⚠️ SÉCURITÉ : Les secrets ci-dessous sont pour le DÉVELOPPEMENT UNIQUEMENT.
/// En production, utiliser des secrets forts et les stocker en variables d'environnement.
/// </summary>
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
        
        // ⚠️ SECRET DE DÉVELOPPEMENT - Changer en production
        var clientSecret = "secret-gestion-personnel-2024";
        var hashedSecret = passwordHasher.Hash(clientSecret);

        context.ClientApplications.Add(new ClientApplication
        {
            Id = Guid.NewGuid(),
            Name = "Gestion du Personnel",
            ClientId = clientId,
            ClientSecret = hashedSecret,
            RedirectUri = "http://localhost:5174/auth/callback",
            PostLogoutRedirectUri = "http://localhost:5174/login",
            
            AllowedScopes = "openid profile email roles offline_access gestion-personnel",
            
            AllowedGrantTypes = "authorization_code refresh_token",
            
            RequirePkce = false,
            RequireConsent = false,  // SSO silencieux - pas de page de consentement
            
            AccessTokenLifetime = 900,
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
        var clientSecret = "secret-tims-2024";
        var hashedSecret = passwordHasher.Hash(clientSecret);

        context.ClientApplications.Add(new ClientApplication
        {
            Id = Guid.NewGuid(),
            Name = "ONEE TIMS",
            ClientId = clientId,
            ClientSecret = hashedSecret,
            RedirectUri = "http://localhost:5175/auth/callback",
            PostLogoutRedirectUri = "http://localhost:5175/login",
            
            AllowedScopes = "openid profile email roles offline_access tims tims_user_id tims_service_id tims_team_id",
            
            AllowedGrantTypes = "authorization_code refresh_token",
            
            RequirePkce = false,
            RequireConsent = false,  // SSO silencieux - pas de page de consentement
            
            AccessTokenLifetime = 3600,
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
        var clientSecret = "secret-eams-2024";
        var hashedSecret = passwordHasher.Hash(clientSecret);

        context.ClientApplications.Add(new ClientApplication
        {
            Id = Guid.NewGuid(),
            Name = "ONEE EAMS",
            ClientId = clientId,
            ClientSecret = hashedSecret,
            RedirectUri = "http://localhost:5173/auth/callback",
            PostLogoutRedirectUri = "http://localhost:5173/login",
            
            AllowedScopes = "openid profile email roles offline_access eams eams_user_id serviceId",
            
            AllowedGrantTypes = "authorization_code refresh_token",
            
            RequirePkce = false,
            RequireConsent = false,  // SSO silencieux - pas de page de consentement
            
            AccessTokenLifetime = 1800,
            RefreshTokenLifetime = 2592000,
            
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
    }
}