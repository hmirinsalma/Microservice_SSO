using System.Security.Claims;

namespace TIMS.API.Extensions;

/// <summary>
/// Helper centralisé pour lire les claims JWT.
///
/// Tous les Controllers doivent utiliser ce helper.
/// Aucun Controller ne doit faire directement :
///   int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
///
/// Résolution :
///   sub (SsoId)  →  tims_user_id (Id local TIMS)
///
/// Lors de l'intégration SSO, seul ce fichier devra être adapté
/// si le format des claims change.
/// </summary>
public static class ClaimsHelper
{
    /// <summary>Retourne l'Id local TIMS de l'utilisateur connecté.</summary>
    public static int GetTimsUserId(ClaimsPrincipal user)
    {
        // Priorité 1 : claim tims_user_id (présent dans StubAuthService et futur SsoAuthService)
        var timsId = user.FindFirstValue("tims_user_id");
        if (!string.IsNullOrEmpty(timsId) && int.TryParse(timsId, out var id))
            return id;

        // Fallback : NameIdentifier standard ASP.NET
        var nameId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(nameId) && int.TryParse(nameId, out var nid))
            return nid;

        throw new UnauthorizedAccessException("Claim tims_user_id introuvable dans le JWT.");
    }

    /// <summary>Retourne le SsoId (claim 'sub') de l'utilisateur connecté.</summary>
    public static string GetSsoId(ClaimsPrincipal user)
        => user.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Claim sub introuvable dans le JWT.");

    /// <summary>Retourne le premier rôle de l'utilisateur depuis le JWT.</summary>
    public static string GetRole(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    /// <summary>Retourne tous les rôles de l'utilisateur depuis le JWT.</summary>
    public static List<string> GetRoles(ClaimsPrincipal user)
        => user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

    /// <summary>Retourne le serviceId depuis le JWT.</summary>
    public static int? GetServiceId(ClaimsPrincipal user)
    {
        var v = user.FindFirstValue("serviceId");
        return int.TryParse(v, out var i) && i > 0 ? i : null;
    }

    /// <summary>Retourne le teamId (équipeId) depuis le JWT.</summary>
    public static int? GetTeamId(ClaimsPrincipal user)
    {
        var v = user.FindFirstValue("teamId");
        return int.TryParse(v, out var i) && i > 0 ? i : null;
    }

    /// <summary>Vérifie si l'utilisateur a un rôle spécifique.</summary>
    public static bool HasRole(ClaimsPrincipal user, string role)
        => user.IsInRole(role);

    /// <summary>Retourne l'email depuis le JWT.</summary>
    public static string? GetEmail(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Email)
        ?? user.FindFirstValue(JwtRegisteredClaimNames.Email);

    // Compatibilité JwtRegisteredClaimNames
    private static class JwtRegisteredClaimNames
    {
        public const string Sub         = "sub";
        public const string Email       = "email";
        public const string GivenName   = "given_name";
        public const string FamilyName  = "family_name";
    }
}
