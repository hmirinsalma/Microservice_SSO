namespace ONEE.SSO.Application.DTOs;

public class RefreshTokenDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }
}