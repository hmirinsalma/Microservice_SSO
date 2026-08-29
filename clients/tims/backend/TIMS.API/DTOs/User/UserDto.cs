namespace TIMS.API.DTOs.User;

/// <summary>DTO métier utilisateur — aucune donnée d'authentification.</summary>
public class UserDto
{
    public int     Id               { get; set; }
    public string? SsoId            { get; set; }
    public string  FirstName        { get; set; } = string.Empty;
    public string  LastName         { get; set; } = string.Empty;
    public string  FullName         => $"{FirstName} {LastName}";
    public string  Email            { get; set; } = string.Empty;
    public string? Phone            { get; set; }
    public string? Poste            { get; set; }
    public string? ProfilePhotoPath { get; set; }
    public string? RoleMetier       { get; set; }
    public bool    IsActive         { get; set; }
    public DateTime CreatedAt       { get; set; }
    public string?  ServiceName     { get; set; }
    public int?     ServiceId       { get; set; }
    public string?  EquipeName      { get; set; }
    public int?     EquipeId        { get; set; }
    public List<string> Roles       { get; set; } = new();
}

public class CreateUserDto
{
    public string  FirstName  { get; set; } = string.Empty;
    public string  LastName   { get; set; } = string.Empty;
    public string  Email      { get; set; } = string.Empty;
    public string? Phone      { get; set; }
    public string? Poste      { get; set; }

    /// <summary>
    /// ⚠️ TEMPORAIRE STUB — Uniquement pour StubCredentials.
    /// Sera supprimé lors de l'intégration SSO.
    /// </summary>
    public string? Password   { get; set; }

    public int     RoleId     { get; set; }
    public string? RoleMetier { get; set; }
    public int?    ServiceId  { get; set; }
    public int?    EquipeId   { get; set; }
}

public class UpdateUserDto
{
    public int?    RoleId     { get; set; }
    public string? RoleMetier { get; set; }
    public int?    ServiceId  { get; set; }
    public int?    EquipeId   { get; set; }
    public bool?   IsActive   { get; set; }
    public string? Poste      { get; set; }
}

public class UpdateProfileDto
{
    public string? Phone { get; set; }
}

/// <summary>
/// ⚠️ TEMPORAIRE STUB — Sera supprimé lors de l'intégration SSO.
/// Le changement de mot de passe sera géré par le microservice SSO.
/// </summary>
public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword     { get; set; } = string.Empty;
}

public class ServiceDto
{
    public int     Id          { get; set; }
    public string  Name        { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool    IsActive    { get; set; }
    public int     UserCount   { get; set; }
    public int     EquipeCount { get; set; }
}

public class CreateServiceDto
{
    public string  Name        { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class EquipeDto
{
    public int     Id          { get; set; }
    public string  Name        { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool    IsActive    { get; set; }
    public int     ServiceId   { get; set; }
    public string? ServiceName { get; set; }
    public int     MemberCount { get; set; }
}

public class CreateEquipeDto
{
    public string  Name        { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int     ServiceId   { get; set; }
}
