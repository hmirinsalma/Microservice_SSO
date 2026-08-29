using System.Security.Claims;
using GestionPersonnel.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace GestionPersonnel.API.Infrastructure;

/// <summary>
/// Utilitaire SSO-Ready pour résoudre l'identité de l'utilisateur connecté.
///
/// Stratégie :
///   Le claim "sub" représente l'identifiant SSO (SsoId).
///   ClaimsHelper fait SsoId → User.Id local via la base de données.
///
/// Phase Stub (actuelle) :
///   sub = User.Id local (entier, converti en string)
///   ResolveLocalUserIdAsync cherche d'abord par SsoId, puis par Id direct (fallback stub).
///
/// Phase SSO (future) :
///   sub = UUID SSO (ex: "keycloak|abc-123")
///   ResolveLocalUserIdAsync cherche uniquement par SsoId.
///   Le fallback stub est supprimé — aucun autre changement dans les Controllers.
/// </summary>
public static class ClaimsHelper
{
    /// <summary>
    /// Retourne le rôle depuis le claim JWT (ClaimTypes.Role).
    /// Ce claim est fourni par le stub ou par le SSO — transparent pour les Controllers.
    /// </summary>
    public static string GetRole(ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.Role)
           ?? throw new UnauthorizedAccessException("Claim de rôle manquant dans le token JWT.");

    /// <summary>
    /// Retourne la valeur brute du claim 'sub' (SsoId côté SSO, UserId local côté stub).
    /// </summary>
    public static string GetSubClaim(ClaimsPrincipal principal)
        => principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
           ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? throw new UnauthorizedAccessException("Claim 'sub' manquant dans le token JWT.");

    /// <summary>
    /// Résout le User.Id local depuis le claim 'sub'.
    ///
    /// Logique :
    ///   1. Lire le claim sub
    ///   2. Chercher User WHERE SsoId = sub (mode SSO)
    ///   3. Fallback : si sub est un entier, chercher User WHERE Id = sub (mode stub)
    ///
    /// Lors de l'intégration SSO : supprimer l'étape 3.
    /// </summary>
    public static async Task<int> ResolveLocalUserIdAsync(
        ClaimsPrincipal principal,
        AppDbContext db,
        CancellationToken ct = default)
    {
        var sub = GetSubClaim(principal);

        // Étape 1 : chercher par SsoId (mode SSO natif)
        var userBySso = await db.Users
            .Where(u => u.SsoId == sub && u.IsActive)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync(ct);

        if (userBySso.HasValue)
            return userBySso.Value;

        // Étape 2 : fallback stub — sub contient un entier local
        // STUB TEMPORAIRE — Supprimé lors de l'intégration SSO
        if (int.TryParse(sub, out var localId))
        {
            var userById = await db.Users
                .Where(u => u.Id == localId && u.IsActive)
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync(ct);

            if (userById.HasValue)
                return userById.Value;
        }

        throw new UnauthorizedAccessException(
            $"Utilisateur introuvable pour le claim sub='{sub}'. Vérifiez le lien SsoId ou User.Id.");
    }
}
