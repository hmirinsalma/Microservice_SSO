using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Persistence.Seed;

public static class RolesSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var rhClient = await context.ClientApplications
            .FirstOrDefaultAsync(c => c.ClientId == "gestion-personnel");

        var timsClient = await context.ClientApplications
            .FirstOrDefaultAsync(c => c.ClientId == "tims-app");

        var eamsClient = await context.ClientApplications
            .FirstOrDefaultAsync(c => c.ClientId == "eams-spa");

        if (rhClient is null || timsClient is null || eamsClient is null)
        {
            throw new InvalidOperationException(
                "Client applications must be seeded before roles."
            );
        }

        await SeedRhRolesAsync(context, rhClient.Id);
        await SeedTimsRolesAsync(context, timsClient.Id);
        await SeedEamsRolesAsync(context, eamsClient.Id);

        await context.SaveChangesAsync();
    }

    // ============================================================
    // RH
    // ============================================================

    private static async Task SeedRhRolesAsync(
        ApplicationDbContext context,
        Guid clientId)
    {
        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "AdministrateurRH",
            "Administrateur de l'application Gestion RH",
            true);

        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "ChefService",
            "Chef de service de l'application Gestion RH",
            true);

        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "Directeur",
            "Directeur de l'application Gestion RH",
            true);

        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "Employe",
            "Employé de l'application Gestion RH",
            false);
    }

    // ============================================================
    // TIMS
    // ============================================================

    private static async Task SeedTimsRolesAsync(
        ApplicationDbContext context,
        Guid clientId)
    {
        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "Administrateur_Technique",
            "Administrateur technique de l'application TIMS",
            true);

        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "Chef_de_Service",
            "Chef de service de l'application TIMS",
            true);

        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "Directeur_Technique",
            "Directeur technique de l'application TIMS",
            true);

        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "Technicien",
            "Technicien de l'application TIMS",
            false);
    }

    // ============================================================
    // EAMS
    // ============================================================

    private static async Task SeedEamsRolesAsync(
        ApplicationDbContext context,
        Guid clientId)
    {
        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "Admin_Patrimoine",
            "Administrateur patrimoine de l'application EAMS",
            true);

        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "Chef_de_Service",
            "Chef de service de l'application EAMS",
            true);

        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "Directeur",
            "Directeur de l'application EAMS",
            true);

        await AddRoleIfNotExistsAsync(
            context,
            clientId,
            "Technicien",
            "Technicien de l'application EAMS",
            false);
    }

    // ============================================================
    // Helper
    // ============================================================

    private static async Task AddRoleIfNotExistsAsync(
        ApplicationDbContext context,
        Guid clientId,
        string name,
        string description,
        bool isSystemRole)
    {
        var exists = await context.Roles
            .AnyAsync(r =>
                r.ClientId == clientId &&
                r.Name == name);

        if (exists)
            return;

        context.Roles.Add(new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            IsSystemRole = isSystemRole,
            ClientId = clientId,
            CreatedAt = DateTime.UtcNow
        });
    }
}