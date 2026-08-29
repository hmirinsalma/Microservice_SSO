using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Domain.Entities;
using ONEE.SSO.Infrastructure.Security;

namespace ONEE.SSO.Infrastructure.Persistence.Seed;

public static class UsersSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Vérifier si un utilisateur admin existe déjà
        var adminExists = await context.Users
            .AnyAsync(u => u.Email == "admin@onee.ma");

        if (adminExists)
        {
            return; // Admin existe déjà
        }

        // Créer le hash du mot de passe Admin@123
        var passwordHasher = new BCryptPasswordHasher();
        var passwordHash = passwordHasher.Hash("Admin@123");

        // Créer l'utilisateur admin
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@onee.ma",
            PasswordHash = passwordHash,
            FirstName = "Admin",
            LastName = "User",
            IsActive = true,
            IsLocked = false,
            FailedLoginAttempts = 0,
            IsEmailVerified = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();

        Console.WriteLine($"== Utilisateur Admin créé : {adminUser.Email} ==");

        // Récupérer le rôle SuperAdmin de l'application gestion-personnel
        var gestPersonnelClient = await context.ClientApplications
            .FirstOrDefaultAsync(c => c.ClientId == "gestion-personnel");

        if (gestPersonnelClient != null)
        {
            // Chercher un rôle admin pour cette application
            var adminRole = await context.Roles
                .FirstOrDefaultAsync(r => 
                    r.ClientId == gestPersonnelClient.Id && 
                    r.Name.Contains("Admin"));

            if (adminRole != null)
            {
                // Assigner le rôle à l'utilisateur
                var userRole = new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    AssignedAt = DateTime.UtcNow
                };

                context.UserRoles.Add(userRole);
                await context.SaveChangesAsync();

                Console.WriteLine($"== Rôle {adminRole.Name} assigné à Admin ==");
            }
        }
    }
}
