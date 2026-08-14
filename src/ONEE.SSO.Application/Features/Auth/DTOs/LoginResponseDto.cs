namespace ONEE.SSO.Application.Features.Auth.DTOs;

public class LoginResponseDto
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime RefreshTokenExpiresAt { get; set; }

    public IEnumerable<string> Roles { get; set; } = new List<string>();
}