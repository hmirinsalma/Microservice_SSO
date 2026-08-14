namespace ONEE.SSO.Application.Features.Auth.DTOs;

public class RefreshTokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
}