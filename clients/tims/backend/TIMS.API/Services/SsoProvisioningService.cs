using TIMS.API.Data;
using TIMS.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TIMS.API.Services;

/// <summary>
/// Service de provisioning automatique des utilisateurs SSO pour TIMS.
/// Crée automatiquement un compte utilisateur lors de la première connexion SSO.
/// </summary>
public class SsoProvisioningService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SsoProvisioningService> _logger;

    public SsoProvisioningService(ApplicationDbContext context, ILogger<SsoProvisioningService> logger)
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

        // Extraire les rôles SSO
        var ssoRoles = ssoUser.FindAll("role")
                              .Union(ssoUser.FindAll(ClaimTypes.Role))
                              .Select(c => c.Value)
                              .ToList();

        _logger.LogInformation($"🔍 [TIMS] Recherche utilisateur SSO: SsoId={ssoId}, Email={email}, Rôles={string.Join(", ", ssoRoles)}");

        // ✅ VALIDATION CRITIQUE: Vérifier que l'utilisateur a au moins UN rôle TIMS valide
        if (!HasValidTimsRole(ssoRoles))
        {
            _logger.LogError($"❌ [TIMS] ACCÈS REFUSÉ: {email} n'a aucun rôle TIMS valide. Rôles SSO: {string.Join(", ", ssoRoles)}");
            throw new UnauthorizedAccessException($"Accès refusé à TIMS: L'utilisateur {email} ne possède aucun rôle TIMS autorisé.");
        }

        // 2. Chercher l'utilisateur par SsoId OU Email
        var user = await _context.Users
            .Include(u => u.Service)
            .Include(u => u.Equipe)
            .FirstOrDefaultAsync(u => u.SsoId == ssoId || u.Email == email);

        if (user != null)
        {
            // Si trouvé par email mais sans SsoId, on met à jour le SsoId
            if (string.IsNullOrEmpty(user.SsoId))
            {
                _logger.LogWarning($"🔄 [TIMS] Utilisateur {email} trouvé sans SsoId, ajout du SsoId...");
                user.SsoId = ssoId;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation($"✅ [TIMS] Utilisateur existant trouvé: {user.Email} (ID: {user.Id})");
            return user;
        }

        // 3. AUTO-PROVISIONING
        _logger.LogWarning($"🆕 [TIMS] AUTO-PROVISIONING: Création automatique de {email}");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 3.1 Mapper les rôles SSO vers RoleMetier TIMS
            var roleMetier = MapSsoRolesToRoleMetier(ssoRoles);
            
            _logger.LogInformation($"🎭 [TIMS] Rôle attribué: {roleMetier} (depuis rôles SSO: {string.Join(", ", ssoRoles)})");

            user = new User
            {
                SsoId = ssoId,
                FirstName = givenName ?? "À renseigner",
                LastName = familyName ?? "À renseigner",
                Email = email,
                RoleMetier = roleMetier, // ✅ Rôle mappé depuis le SSO
                ServiceId = 1, // Service par défaut (ajustez selon votre base)
                EquipeId = 1,  // Équipe par défaut (ajustez selon votre base)
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"✅ [TIMS] User créé: ID={user.Id}, Email={user.Email}, RoleMetier={user.RoleMetier}");

            // TIMS n'a pas besoin de StubCredentials - PasswordHash est déjà NULL dans Users
            // Les utilisateurs SSO s'authentifient via JWT uniquement

            await transaction.CommitAsync();

            _logger.LogInformation($"🎉 [TIMS] AUTO-PROVISIONING RÉUSSI pour {email}!");

            // Recharger avec relations
            user = await _context.Users
                .Include(u => u.Service)
                .Include(u => u.Equipe)
                .FirstAsync(u => u.Id == user.Id);

            return user;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, $"❌ [TIMS] Erreur lors de l'auto-provisioning de {email}");
            throw new Exception($"Échec du provisioning automatique: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Vérifie si l'utilisateur possède au moins un rôle TIMS valide.
    /// ✅ FIX CRITIQUE: Vérifie maintenant les rôles QUALIFIÉS avec @tims-app
    /// </summary>
    private bool HasValidTimsRole(List<string> ssoRoles)
    {
        if (ssoRoles == null || !ssoRoles.Any())
            return false;

        // ✅ SECURITY FIX: On accepte UNIQUEMENT les rôles qualifiés avec @tims-app
        // Format attendu: "RoleName@tims-app"
        // Exemple: "Technicien@tims-app", "Administrateur_Technique@tims-app"
        
        var timsRoles = ssoRoles
            .Where(r => r.EndsWith("@tims-app", StringComparison.OrdinalIgnoreCase))
            .Select(r => r.Split('@')[0].ToLowerInvariant())
            .ToHashSet();

        if (!timsRoles.Any())
        {
            _logger.LogWarning($"❌ [TIMS] Aucun rôle qualifié @tims-app trouvé dans: {string.Join(", ", ssoRoles)}");
            return false;
        }

        // Rôles TIMS autorisés (nom du rôle uniquement, sans le @tims-app)
        var validTimsRoles = new[]
        {
            "administrateur_technique",
            "admin",
            "administrator",
            "superadmin",
            "chef_equipe",
            "chefequipe",
            "responsable",
            "manager",
            "chef",
            "directeurtechnique",
            "technicien",
            "technician",
            "operateur",
            "operator",
            "superviseur"
        };

        var hasValidRole = validTimsRoles.Any(validRole => timsRoles.Contains(validRole));
        
        if (hasValidRole)
        {
            _logger.LogInformation($"✅ [TIMS] Rôles TIMS valides détectés: {string.Join(", ", timsRoles)}");
        }
        
        return hasValidRole;
    }

    /// <summary>
    /// Mappe les rôles SSO vers RoleMetier TIMS.
    /// ✅ FIX CRITIQUE: Traite maintenant les rôles qualifiés avec @tims-app
    /// LANCE UNE EXCEPTION si aucun rôle TIMS valide n'est trouvé (sécurité).
    /// </summary>
    private string MapSsoRolesToRoleMetier(List<string> ssoRoles)
    {
        if (ssoRoles == null || !ssoRoles.Any())
        {
            _logger.LogError("❌ [TIMS] REFUS: Aucun rôle SSO trouvé pour cet utilisateur");
            throw new UnauthorizedAccessException("Accès refusé: Aucun rôle TIMS autorisé pour cet utilisateur");
        }

        // ✅ Extraire UNIQUEMENT les rôles TIMS qualifiés avec @tims-app
        var timsRoles = ssoRoles
            .Where(r => r.EndsWith("@tims-app", StringComparison.OrdinalIgnoreCase))
            .Select(r => r.Split('@')[0].ToLowerInvariant())
            .ToList();

        if (!timsRoles.Any())
        {
            _logger.LogError($"❌ [TIMS] REFUS: Aucun rôle @tims-app trouvé dans: {string.Join(", ", ssoRoles)}");
            throw new UnauthorizedAccessException("Accès refusé: Aucun rôle TIMS autorisé pour cet utilisateur");
        }

        // Mapping des rôles TIMS (priorité décroissante)
        
        // 1. ADMINISTRATEUR_TECHNIQUE (priorité maximale)
        if (timsRoles.Contains("admin") || 
            timsRoles.Contains("administrator") || 
            timsRoles.Contains("superadmin") ||
            timsRoles.Contains("administrateur_technique"))
        {
            _logger.LogInformation("🔐 [TIMS] Rôle Administrateur_Technique détecté");
            return "Administrateur_Technique"; // ✅ FIX: Correspond au dashboard
        }

        // 2. CHEF D'ÉQUIPE
        if (timsRoles.Contains("responsable") || 
            timsRoles.Contains("manager") || 
            timsRoles.Contains("chef") ||
            timsRoles.Contains("directeurtechnique") ||
            timsRoles.Contains("chef_equipe") ||
            timsRoles.Contains("chefequipe") ||
            timsRoles.Contains("superviseur"))
        {
            _logger.LogInformation("👔 [TIMS] Rôle Chef d'Équipe détecté");
            return "Chef d'Équipe";
        }

        // 3. TECHNICIEN
        if (timsRoles.Contains("technicien") || 
            timsRoles.Contains("technician"))
        {
            _logger.LogInformation("🔧 [TIMS] Rôle Technicien détecté");
            return "Technicien";
        }

        // 4. OPÉRATEUR
        if (timsRoles.Contains("operateur") || 
            timsRoles.Contains("operator"))
        {
            _logger.LogInformation("⚙️ [TIMS] Rôle Opérateur détecté");
            return "Opérateur";
        }

        // Par défaut: AUCUN rôle TIMS trouvé → REFUS
        _logger.LogError($"❌ [TIMS] REFUS: Aucun rôle TIMS valide trouvé. Rôles SSO reçus: {string.Join(", ", ssoRoles)}");
        throw new UnauthorizedAccessException($"Accès refusé à TIMS: L'utilisateur ne possède aucun rôle TIMS autorisé (Rôles: {string.Join(", ", ssoRoles)})");
    }

    /// <summary>
    /// Met à jour les informations de l'utilisateur à partir du SSO.
    /// </summary>
    public async Task UpdateUserFromSsoAsync(User user, ClaimsPrincipal ssoUser)
    {
        var email = ssoUser.FindFirst("email")?.Value;
        var givenName = ssoUser.FindFirst("given_name")?.Value;
        var familyName = ssoUser.FindFirst("family_name")?.Value;

        bool hasChanges = false;

        if (!string.IsNullOrEmpty(email) && user.Email != email)
        {
            user.Email = email;
            hasChanges = true;
        }

        if (!string.IsNullOrEmpty(givenName) && user.FirstName != givenName)
        {
            user.FirstName = givenName;
            hasChanges = true;
        }

        if (!string.IsNullOrEmpty(familyName) && user.LastName != familyName)
        {
            user.LastName = familyName;
            hasChanges = true;
        }

        if (hasChanges)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"🔄 [TIMS] Informations synchronisées pour {user.Email}");
        }
    }
}
