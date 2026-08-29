using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Application.DTOs.User;

/// <summary>DTO de consultation d'un utilisateur — pas de données d'authentification.</summary>
public record UserDto(
    Guid Id,
    string Nom,
    string Prenom,
    string Email,
    string Telephone,
    string Poste,
    string? PhotoUrl,
    UserRole Role,
    Guid? ServiceId,
    string? ServiceNom,
    bool IsActive
);

/// <summary>
/// Mise à jour du profil personnel — uniquement les champs non sensibles.
/// Le mot de passe est géré par le SSO.
/// </summary>
public record UpdateProfileRequest(string? Telephone, string? PhotoUrl);

/// <summary>
/// Création d'un utilisateur métier local.
/// Aucun mot de passe — le compte d'authentification est créé côté SSO.
/// Le SsoId sera renseigné lors de la liaison avec le SSO.
/// </summary>
public record CreateUserRequest(
    string Nom,
    string Prenom,
    string Email,
    string Telephone,
    string Poste,
    UserRole Role,
    Guid? ServiceId,
    string? SsoId = null
);

/// <summary>Modification des informations métier d'un utilisateur.</summary>
public record UpdateUserRequest(
    string Nom,
    string Prenom,
    string Telephone,
    string Poste,
    UserRole Role,
    Guid? ServiceId,
    bool IsActive
);
