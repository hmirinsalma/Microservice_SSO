namespace TIMS.API.DTOs.Auth;

public class LoginDto
{
    public string Email    { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string      Token     { get; set; } = string.Empty;
    public DateTime    ExpiresAt { get; set; }
    public UserInfoDto User      { get; set; } = null!;
}

public class UserInfoDto
{
    public int     Id               { get; set; }
    /// <summary>Identifiant SSO (claim 'sub'). Préfixé 'stub-' en mode Stub.</summary>
    public string? SsoId            { get; set; }
    public string  FirstName        { get; set; } = string.Empty;
    public string  LastName         { get; set; } = string.Empty;
    public string  Email            { get; set; } = string.Empty;
    public string? ProfilePhotoPath { get; set; }
    public List<string> Roles       { get; set; } = new();
    public string? ServiceName      { get; set; }
    public int?    ServiceId        { get; set; }
    public string? EquipeName       { get; set; }
    public int?    EquipeId         { get; set; }
}
