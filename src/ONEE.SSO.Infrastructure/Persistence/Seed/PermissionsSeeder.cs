using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Persistence.Seed;

public static class PermissionsSeeder
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
                "Client applications must be seeded before permissions."
            );
        }

        await SeedRhPermissionsAsync(context, rhClient.Id);
        await SeedTimsPermissionsAsync(context, timsClient.Id);
        await SeedEamsPermissionsAsync(context, eamsClient.Id);

        await context.SaveChangesAsync();
    }

    // ============================================================
    // RH
    // ============================================================

    private static async Task SeedRhPermissionsAsync(
        ApplicationDbContext context,
        Guid clientId)
    {
        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_READ",
            "Lire les utilisateurs",
            "Autorise la lecture des utilisateurs");

        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_CREATE",
            "Créer un utilisateur",
            "Autorise la création des utilisateurs");

        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_UPDATE",
            "Modifier un utilisateur",
            "Autorise la modification des utilisateurs");

        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_DELETE",
            "Supprimer un utilisateur",
            "Autorise la suppression des utilisateurs");
    }

    // ============================================================
    // TIMS
    // ============================================================

    private static async Task SeedTimsPermissionsAsync(
        ApplicationDbContext context,
        Guid clientId)
    {
        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_READ",
            "Lire les utilisateurs",
            "Autorise la lecture des utilisateurs");

        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_CREATE",
            "Créer un utilisateur",
            "Autorise la création des utilisateurs");

        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_UPDATE",
            "Modifier un utilisateur",
            "Autorise la modification des utilisateurs");

        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_DELETE",
            "Supprimer un utilisateur",
            "Autorise la suppression des utilisateurs");
    }

    // ============================================================
    // EAMS
    // ============================================================

    private static async Task SeedEamsPermissionsAsync(
        ApplicationDbContext context,
        Guid clientId)
    {
        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_READ",
            "Lire les utilisateurs",
            "Autorise la lecture des utilisateurs");

        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_CREATE",
            "Créer un utilisateur",
            "Autorise la création des utilisateurs");

        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_UPDATE",
            "Modifier un utilisateur",
            "Autorise la modification des utilisateurs");

        await AddPermissionIfNotExistsAsync(
            context,
            clientId,
            "USER_DELETE",
            "Supprimer un utilisateur",
            "Autorise la suppression des utilisateurs");
    }

    // ============================================================
    // Helper
    // ============================================================

    private static async Task AddPermissionIfNotExistsAsync(
        ApplicationDbContext context,
        Guid clientId,
        string code,
        string name,
        string description)
    {
        var exists = await context.Permissions
            .AnyAsync(p =>
                p.ClientId == clientId &&
                p.Code == code);

        if (exists)
            return;

        context.Permissions.Add(new Permission
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            Description = description,
            ClientId = clientId,
            CreatedAt = DateTime.UtcNow
        });
    }
}