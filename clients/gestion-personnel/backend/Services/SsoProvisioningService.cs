using GestionPersonnel.API.Data;
using GestionPersonnel.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GestionPersonnel.API.Services;

/// <summary>
/// Service de provisioning automatique des utilisateurs SSO.
/// Crée automatiquement un compte utilisateur + fiche employé lors de la première connexion SSO.
/// </summary>
public class SsoProvisioningService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SsoProvisioningService> _logger;

    public SsoProvisioningService(AppDbContext context, ILogger<SsoProvisioningService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Récupère ou crée automatiquement un utilisateur à partir des claims SSO.
    /// </summary>
    /// <param name="ssoUser">ClaimsPrincipal provenant du SSO (après authentification)</param>
    /// <returns>L'utilisateur existant ou nouvellement créé</returns>
    public async Task<User> GetOrCreateUserFromSsoAsync(ClaimsPrincipal ssoUser)
    {
        // 1. Extraire les claims SSO
        var ssoId = ssoUser.FindFirst("sub")?.Value
                    ?? ssoUser.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new Exception("Claim 'sub' manquant dans le token SSO");

        var email = ssoUser.FindFirst("email")?.Value
                    ?? ssoUser.FindFirst(ClaimTypes.Email)?.Value
                    ?? throw new Exception("Claim 'email' manquant dans le token SSO");

        var givenName = ssoUser.FindFirst("given_name")?.Value
                       ?? ssoUser.FindFirst(ClaimTypes.GivenName)?.Value;

        var familyName = ssoUser.FindFirst("family_name")?.Value
                        ?? ssoUser.FindFirst(ClaimTypes.Surname)?.Value;

        // Extraire les rôles SSO
        var ssoRoles = ssoUser.FindAll("role")
                              .Union(ssoUser.FindAll(ClaimTypes.Role))
                              .Select(c => c.Value)
                              .ToList();

        _logger.LogInformation($"🔍 Recherche utilisateur SSO: SsoId={ssoId}, Email={email}, Rôles={string.Join(", ", ssoRoles)}");

        // 2. Chercher l'utilisateur par SsoId
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Employe)
            .FirstOrDefaultAsync(u => u.SsoId == ssoId);

        if (user != null)
        {
            _logger.LogInformation($"✅ Utilisateur existant trouvé: {user.Email} (ID: {user.Id})");
            return user;
        }

        // 3. AUTO-PROVISIONING: L'utilisateur n'existe pas, on le crée!
        _logger.LogWarning($"🆕 AUTO-PROVISIONING: Création automatique de l'utilisateur {email}");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 3.1 Mapper les rôles SSO vers les RoleId de l'application
            var roleId = MapSsoRolesToRoleId(ssoRoles);
            
            _logger.LogInformation($"🎭 Rôle attribué: RoleId={roleId} (depuis rôles SSO: {string.Join(", ", ssoRoles)})");

            // 3.2 Créer le compte utilisateur
            user = new User
            {
                SsoId = ssoId,
                Username = email.Split('@')[0], // "mohamed.hassan" from email
                Email = email,
                RoleId = roleId, // ✅ Rôle mappé depuis le SSO
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Sauvegarder pour obtenir l'ID

            _logger.LogInformation($"✅ User créé: ID={user.Id}, Email={user.Email}, RoleId={user.RoleId}");

            // 3.2 Créer la fiche employé automatiquement
            var employe = new Employe
            {
                Matricule = $"EMP-{user.Id:D4}", // EMP-0001, EMP-0002, etc.
                Nom = familyName ?? "À renseigner",
                Prenom = givenName ?? "À renseigner",
                Email = email,
                Poste = "À définir",
                DirectionId = 1, // Direction par défaut (ajustez selon votre base)
                ServiceId = 1,   // Service par défaut (ajustez selon votre base)
                Statut = StatutEmploye.Actif,
                DateEmbauche = DateTime.Today,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Employes.Add(employe);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"✅ Fiche employé créée: Matricule={employe.Matricule}");

            // 3.3 Créer un credential stub (pour compatibilité)
            var stubCredential = new StubCredential
            {
                UserId = user.Id,
                PasswordHash = "$2a$11$STUB_NO_PASSWORD_NEEDED" // Hash factice
            };

            _context.StubCredentials.Add(stubCredential);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            _logger.LogInformation($"🎉 AUTO-PROVISIONING RÉUSSI pour {email}!");

            // Recharger l'utilisateur avec ses relations
            user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Employe)
                .FirstAsync(u => u.Id == user.Id);

            return user;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, $"❌ Erreur lors de l'auto-provisioning de {email}");
            throw new Exception($"Échec du provisioning automatique: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mappe les rôles SSO vers les RoleId de l'application RH.
    /// ✅ FIX CRITIQUE: Traite maintenant les rôles qualifiés avec @gestion-personnel
    /// Priorité: Admin > Manager > Employé
    /// </summary>
    private int MapSsoRolesToRoleId(List<string> ssoRoles)
    {
        if (ssoRoles == null || !ssoRoles.Any())
        {
            _logger.LogWarning("⚠️ Aucun rôle SSO trouvé, attribution du rôle Employé par défaut");
            return 4; // Employé par défaut
        }

        // ✅ Extraire UNIQUEMENT les rôles RH qualifiés avec @gestion-personnel
        var rhRoles = ssoRoles
            .Where(r => r.EndsWith("@gestion-personnel", StringComparison.OrdinalIgnoreCase))
            .Select(r => r.Split('@')[0].ToLowerInvariant())
            .ToList();

        if (!rhRoles.Any())
        {
            _logger.LogWarning($"⚠️ [RH] Aucun rôle @gestion-personnel trouvé dans: {string.Join(", ", ssoRoles)}, attribution du rôle Employé par défaut");
            return 4; // Employé par défaut
        }

        // Mapping avec priorité décroissante
        if (rhRoles.Contains("admin") || 
            rhRoles.Contains("administrator") || 
            rhRoles.Contains("administrateurrh") ||
            rhRoles.Contains("adminrh"))
        {
            _logger.LogInformation("🔐 [RH] Rôle Admin détecté");
            return 1; // Admin
        }

        if (rhRoles.Contains("rh_manager") || 
            rhRoles.Contains("manager") || 
            rhRoles.Contains("responsable") || 
            rhRoles.Contains("directeur"))
        {
            _logger.LogInformation("👔 [RH] Rôle Manager détecté");
            return 2; // Manager
        }

        if (rhRoles.Contains("rh_user") || 
            rhRoles.Contains("user") || 
            rhRoles.Contains("chefservice") ||
            rhRoles.Contains("employe"))
        {
            _logger.LogInformation("👤 [RH] Rôle Utilisateur RH détecté");
            return 3; // Utilisateur RH
        }

        // Par défaut: Employé
        _logger.LogInformation("👷 [RH] Rôle Employé par défaut (rôles RH: {Roles})", string.Join(", ", rhRoles));
        return 4; // Employé
    }

    /// <summary>
    /// Met à jour les informations de l'utilisateur à partir du SSO (synchronisation).
    /// </summary>
    public async Task UpdateUserFromSsoAsync(User user, ClaimsPrincipal ssoUser)
    {
        var email = ssoUser.FindFirst("email")?.Value;
        var givenName = ssoUser.FindFirst("given_name")?.Value;
        var familyName = ssoUser.FindFirst("family_name")?.Value;

        bool hasChanges = false;

        // Synchroniser l'email si changé
        if (!string.IsNullOrEmpty(email) && user.Email != email)
        {
            user.Email = email;
            hasChanges = true;
        }

        // Synchroniser la fiche employé si elle existe
        if (user.Employe != null)
        {
            if (!string.IsNullOrEmpty(givenName) && user.Employe.Prenom != givenName)
            {
                user.Employe.Prenom = givenName;
                hasChanges = true;
            }

            if (!string.IsNullOrEmpty(familyName) && user.Employe.Nom != familyName)
            {
                user.Employe.Nom = familyName;
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"🔄 Informations synchronisées pour {user.Email}");
        }
    }
}
