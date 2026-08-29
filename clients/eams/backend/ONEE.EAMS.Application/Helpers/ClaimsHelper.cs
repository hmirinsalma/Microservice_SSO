using System.Security.Claims;
using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Application.Helpers;

/// <summary>
/// Helpers d'extraction des claims JWT.
///
/// Convention de claims — doit être respectée par le futur microservice SSO :
///
///   "eams_user_id"  → Guid interne EAMS de l'utilisateur (clé primaire de la table Users)
///   ClaimTypes.Role → Rôle métier (Admin_Patrimoine, Directeur, Chef_de_Service, Technicien)
///   "serviceId"     → Guid du service EAMS (nullable)
///   "sub"           → Identifiant SSO externe (SsoId)
///
/// Pourquoi "eams_user_id" et pas "sub" ?
///   Le claim standard "sub" contiendra l'identifiant SSO externe lors de l'intégration.
///   L'application EAMS a besoin de l'Id interne EAMS pour ses requêtes EF Core.
///   Ce claim custom doit être inclus dans le JWT émis par le SSO.
/// </summary>
public static class ClaimsHelper
{
    /// <summary>
    /// Retourne l'identifiant interne EAMS de l'utilisateur connecté.
    /// Le SSO doit inclure le claim "eams_user_id" dans le JWT qu'il émet.
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var val = user.FindFirst("eams_user_id")?.Value
               ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value // fallback compatibilité
               ?? throw new InvalidOperationException("Claim 'eams_user_id' manquant dans le JWT.");
        return Guid.Parse(val);
    }

    /// <summary>
    /// Retourne le rôle métier de l'utilisateur connecté, extrait du JWT.
    /// Ce rôle provient du SSO — jamais de la base de données pour l'autorisation.
    /// </summary>
    public static UserRole GetRole(this ClaimsPrincipal user)
    {
        var val = user.FindFirst(ClaimTypes.Role)?.Value
            ?? throw new InvalidOperationException("Claim 'role' manquant dans le JWT.");
        return Enum.Parse<UserRole>(val);
    }

    /// <summary>Retourne le ServiceId métier depuis le JWT.</summary>
    public static Guid? GetServiceId(this ClaimsPrincipal user)
    {
        var val = user.FindFirst("serviceId")?.Value;
        return string.IsNullOrEmpty(val) ? null : Guid.Parse(val);
    }

    /// <summary>
    /// Retourne l'identifiant SSO externe (sub claim).
    /// Null en phase stub — sera renseigné par le vrai SSO.
    /// </summary>
    public static string? GetSsoId(this ClaimsPrincipal user)
        => user.FindFirst("sub")?.Value;
}
