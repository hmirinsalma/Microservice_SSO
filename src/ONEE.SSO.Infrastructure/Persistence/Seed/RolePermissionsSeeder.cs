using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Persistence.Seed;

public static class RolePermissionsSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var rhClient = await context.ClientApplications
            .FirstOrDefaultAsync(c => c.ClientId == "rh-client");

        var timsClient = await context.ClientApplications
            .FirstOrDefaultAsync(c => c.ClientId == "tims-client");

        var eamsClient = await context.ClientApplications
            .FirstOrDefaultAsync(c => c.ClientId == "eams-client");

        if (rhClient is null || timsClient is null || eamsClient is null)
        {
            throw new InvalidOperationException(
                "Client applications must be seeded before role permissions."
            );
        }

        await SeedRhRolePermissionsAsync(context, rhClient.Id);
        await SeedTimsRolePermissionsAsync(context, timsClient.Id);
        await SeedEamsRolePermissionsAsync(context, eamsClient.Id);

        await context.SaveChangesAsync();
    }

    // ============================================================
    // RH
    // ============================================================

    private static async Task SeedRhRolePermissionsAsync(
        ApplicationDbContext context,
        Guid clientId)
    {
        var administrateur = await GetRoleAsync(
            context,
            clientId,
            "AdministrateurRH");

        var directeur = await GetRoleAsync(
            context,
            clientId,
            "Directeur");

        var chefService = await GetRoleAsync(
            context,
            clientId,
            "ChefService");

        var employe = await GetRoleAsync(
            context,
            clientId,
            "Employe");

        if (administrateur is null ||
            directeur is null ||
            chefService is null ||
            employe is null)
        {
            throw new InvalidOperationException(
                "RH roles must be seeded before role permissions."
            );
        }

        // AdministrateurRH
        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_READ");

        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_CREATE");

        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_UPDATE");

        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_DELETE");

        // Directeur
        await AddPermissionToRoleAsync(
            context, directeur.Id, clientId, "USER_READ");

        await AddPermissionToRoleAsync(
            context, directeur.Id, clientId, "USER_CREATE");

        await AddPermissionToRoleAsync(
            context, directeur.Id, clientId, "USER_UPDATE");

        // ChefService
        await AddPermissionToRoleAsync(
            context, chefService.Id, clientId, "USER_READ");

        await AddPermissionToRoleAsync(
            context, chefService.Id, clientId, "USER_CREATE");

        await AddPermissionToRoleAsync(
            context, chefService.Id, clientId, "USER_UPDATE");

        // Employe
        await AddPermissionToRoleAsync(
            context, employe.Id, clientId, "USER_READ");
    }

    // ============================================================
    // TIMS
    // ============================================================

    private static async Task SeedTimsRolePermissionsAsync(
        ApplicationDbContext context,
        Guid clientId)
    {
        var administrateur = await GetRoleAsync(
            context,
            clientId,
            "Administrateur_Technique");

        var directeur = await GetRoleAsync(
            context,
            clientId,
            "Directeur_Technique");

        var chefService = await GetRoleAsync(
            context,
            clientId,
            "Chef_de_Service");

        var technicien = await GetRoleAsync(
            context,
            clientId,
            "Technicien");

        if (administrateur is null ||
            directeur is null ||
            chefService is null ||
            technicien is null)
        {
            throw new InvalidOperationException(
                "TIMS roles must be seeded before role permissions."
            );
        }

        // Administrateur_Technique
        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_READ");

        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_CREATE");

        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_UPDATE");

        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_DELETE");

        // Directeur_Technique
        await AddPermissionToRoleAsync(
            context, directeur.Id, clientId, "USER_READ");

        await AddPermissionToRoleAsync(
            context, directeur.Id, clientId, "USER_CREATE");

        await AddPermissionToRoleAsync(
            context, directeur.Id, clientId, "USER_UPDATE");

        // Chef_de_Service
        await AddPermissionToRoleAsync(
            context, chefService.Id, clientId, "USER_READ");

        await AddPermissionToRoleAsync(
            context, chefService.Id, clientId, "USER_CREATE");

        await AddPermissionToRoleAsync(
            context, chefService.Id, clientId, "USER_UPDATE");

        // Technicien
        await AddPermissionToRoleAsync(
            context, technicien.Id, clientId, "USER_READ");
    }

    // ============================================================
    // EAMS
    // ============================================================

    private static async Task SeedEamsRolePermissionsAsync(
        ApplicationDbContext context,
        Guid clientId)
    {
        var administrateur = await GetRoleAsync(
            context,
            clientId,
            "Admin_Patrimoine");

        var directeur = await GetRoleAsync(
            context,
            clientId,
            "Directeur");

        var chefService = await GetRoleAsync(
            context,
            clientId,
            "Chef_de_Service");

        var technicien = await GetRoleAsync(
            context,
            clientId,
            "Technicien");

        if (administrateur is null ||
            directeur is null ||
            chefService is null ||
            technicien is null)
        {
            throw new InvalidOperationException(
                "EAMS roles must be seeded before role permissions."
            );
        }

        // Admin_Patrimoine
        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_READ");

        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_CREATE");

        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_UPDATE");

        await AddPermissionToRoleAsync(
            context, administrateur.Id, clientId, "USER_DELETE");

        // Directeur
        await AddPermissionToRoleAsync(
            context, directeur.Id, clientId, "USER_READ");

        await AddPermissionToRoleAsync(
            context, directeur.Id, clientId, "USER_CREATE");

        await AddPermissionToRoleAsync(
            context, directeur.Id, clientId, "USER_UPDATE");

        // Chef_de_Service
        await AddPermissionToRoleAsync(
            context, chefService.Id, clientId, "USER_READ");

        await AddPermissionToRoleAsync(
            context, chefService.Id, clientId, "USER_CREATE");

        await AddPermissionToRoleAsync(
            context, chefService.Id, clientId, "USER_UPDATE");

        // Technicien
        await AddPermissionToRoleAsync(
            context, technicien.Id, clientId, "USER_READ");
    }

    // ============================================================
    // GET ROLE
    // ============================================================

    private static async Task<Role?> GetRoleAsync(
        ApplicationDbContext context,
        Guid clientId,
        string roleName)
    {
        return await context.Roles
            .FirstOrDefaultAsync(r =>
                r.ClientId == clientId &&
                r.Name == roleName);
    }

    // ============================================================
    // ADD PERMISSION TO ROLE
    // ============================================================

    private static async Task AddPermissionToRoleAsync(
        ApplicationDbContext context,
        Guid roleId,
        Guid clientId,
        string permissionCode)
    {
        var permission = await context.Permissions
            .FirstOrDefaultAsync(p =>
                p.ClientId == clientId &&
                p.Code == permissionCode);

        if (permission is null)
        {
            throw new InvalidOperationException(
                $"Permission '{permissionCode}' not found for client '{clientId}'."
            );
        }

        var exists = await context.RolePermissions
            .AnyAsync(rp =>
                rp.RoleId == roleId &&
                rp.PermissionId == permission.Id);

        if (exists)
            return;

        context.RolePermissions.Add(new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            PermissionId = permission.Id
        });
    }
}