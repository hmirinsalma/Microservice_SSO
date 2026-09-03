using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Domain.Entities;
using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Application.Services;

/// <summary>
/// Service de provisioning automatique des utilisateurs SSO pour EAMS.
/// Crée automatiquement un compte utilisateur lors de la première connexion SSO.
/// </summary>
public class SsoProvisioningService
{
    private readonly IAppDbContext _context;
    private readonly ILogger<SsoProvisioningService> _logger;

    public SsoProvisioningService(IAppDbContext context, ILogger<SsoProvisioningService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Récupère ou crée automatiquement un utilisateur à partir des claims SSO.
    /// </summary>
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

        var phoneNumber = ssoUser.FindFirst("phone_number")?.Value
                         ?? ssoUser.FindFirst(ClaimTypes.MobilePhone)?.Value;

        // Extraire les rôles SSO
        var ssoRoles = ssoUser.FindAll("role")
                              .Union(ssoUser.FindAll(ClaimTypes.Role))
                              .Select(c => c.Value)
                              .ToList();

        _logger.LogInformation($"🔍 [EAMS] Recherche utilisateur SSO: SsoId={ssoId}, Email={email}, Rôles={string.Join(", ", ssoRoles)}");

        // 2. Chercher l'utilisateur par SsoId
        var user = await _context.Users
            .Include(u => u.Service)
            .FirstOrDefaultAsync(u => u.SsoId == ssoId);

        if (user != null)
        {
            _logger.LogInformation($"✅ [EAMS] Utilisateur existant trouvé: {user.Email} (ID: {user.Id})");
            return user;
        }

        // 3. AUTO-PROVISIONING
        _logger.LogWarning($"🆕 [EAMS] AUTO-PROVISIONING: Création automatique de {email}");

        try
        {
            // 3.1 Mapper les rôles SSO vers Role EAMS (string)
            var role = MapSsoRolesToRole(ssoRoles);
            
            _logger.LogInformation($"🎭 [EAMS] Rôle attribué: {role} (depuis rôles SSO: {string.Join(", ", ssoRoles)})");

            user = new User
            {
                Id = Guid.NewGuid(),
                SsoId = ssoId,
                Prenom = givenName ?? "À renseigner",
                Nom = familyName ?? "À renseigner",
                Email = email,
                Telephone = phoneNumber ?? "À renseigner",
                Poste = "À définir",
                Role = role, // ✅ Rôle mappé depuis le SSO (string)
                ServiceId = null, // Pas de service par défaut (sera assigné par l'admin)
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"✅ [EAMS] User créé: ID={user.Id}, Email={user.Email}, Role={user.Role}");
            _logger.LogInformation($"🎉 [EAMS] AUTO-PROVISIONING RÉUSSI pour {email}!");

            // Recharger avec relations
            user = await _context.Users
                .Include(u => u.Service)
                .FirstAsync(u => u.Id == user.Id);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ [EAMS] Erreur lors de l'auto-provisioning de {email}");
            throw new Exception($"Échec du provisioning automatique: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Mappe les rôles SSO vers Role EAMS (string).
    /// ✅ FIX CRITIQUE: Traite maintenant les rôles qualifiés avec @eams-spa
    /// </summary>
    private string MapSsoRolesToRole(List<string> ssoRoles)
    {
        if (ssoRoles == null || !ssoRoles.Any())
        {
            _logger.LogWarning("⚠️ [EAMS] Aucun rôle SSO trouvé, attribution du rôle Technicien par défaut");
            return "Technicien";
        }

        // ✅ Extraire UNIQUEMENT les rôles EAMS qualifiés avec @eams-spa
        var eamsRoles = ssoRoles
            .Where(r => r.EndsWith("@eams-spa", StringComparison.OrdinalIgnoreCase))
            .Select(r => r.Split('@')[0].ToLowerInvariant())
            .ToList();

        if (!eamsRoles.Any())
        {
            _logger.LogWarning($"⚠️ [EAMS] Aucun rôle @eams-spa trouvé dans: {string.Join(", ", ssoRoles)}, attribution du rôle Technicien par défaut");
            return "Technicien";
        }

        // Mapping des rôles EAMS (priorité décroissante)
        
        // 1. ADMIN PATRIMOINE (priorité maximale)
        if (eamsRoles.Contains("admin") || 
            eamsRoles.Contains("administrator") || 
            eamsRoles.Contains("admin_patrimoine") ||
            eamsRoles.Contains("adminpatrimoine"))
        {
            _logger.LogInformation("🔐 [EAMS] Rôle Admin_Patrimoine détecté");
            return "Admin_Patrimoine";
        }

        // 2. DIRECTEUR
        if (eamsRoles.Contains("directeur") || 
            eamsRoles.Contains("director") ||
            eamsRoles.Contains("directeurtechnique"))
        {
            _logger.LogInformation("🏢 [EAMS] Rôle Directeur détecté");
            return "Directeur";
        }

        // 3. CHEF DE SERVICE
        if (eamsRoles.Contains("chef_de_service") || 
            eamsRoles.Contains("chef") || 
            eamsRoles.Contains("manager") ||
            eamsRoles.Contains("chefdeservice"))
        {
            _logger.LogInformation("👔 [EAMS] Rôle Chef_de_Service détecté");
            return "Chef_de_Service";
        }

        // 4. TECHNICIEN
        if (eamsRoles.Contains("technicien") || 
            eamsRoles.Contains("technician"))
        {
            _logger.LogInformation("🔧 [EAMS] Rôle Technicien détecté");
            return "Technicien";
        }

        // Par défaut
        _logger.LogInformation("🔧 [EAMS] Rôle Technicien par défaut (rôles EAMS: {Roles})", string.Join(", ", eamsRoles));
        return "Technicien";
    }

    /// <summary>
    /// Met à jour les informations de l'utilisateur à partir du SSO (synchronisation).
    /// </summary>
    public async Task UpdateUserFromSsoAsync(User user, ClaimsPrincipal ssoUser)
    {
        var email = ssoUser.FindFirst("email")?.Value;
        var givenName = ssoUser.FindFirst("given_name")?.Value;
        var familyName = ssoUser.FindFirst("family_name")?.Value;
        var phoneNumber = ssoUser.FindFirst("phone_number")?.Value;

        bool hasChanges = false;

        if (!string.IsNullOrEmpty(email) && user.Email != email)
        {
            user.Email = email;
            hasChanges = true;
        }

        if (!string.IsNullOrEmpty(givenName) && user.Prenom != givenName)
        {
            user.Prenom = givenName;
            hasChanges = true;
        }

        if (!string.IsNullOrEmpty(familyName) && user.Nom != familyName)
        {
            user.Nom = familyName;
            hasChanges = true;
        }

        if (!string.IsNullOrEmpty(phoneNumber) && user.Telephone != phoneNumber)
        {
            user.Telephone = phoneNumber;
            hasChanges = true;
        }

        if (hasChanges)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"🔄 [EAMS] Informations synchronisées pour {user.Email}");
        }
    }
}
