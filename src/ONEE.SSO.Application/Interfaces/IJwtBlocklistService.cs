namespace ONEE.SSO.Application.Interfaces;

public interface IJwtBlocklistService
{
    Task RevokeTokenAsync(string jti, DateTime expiration);
    Task<bool> IsTokenRevokedAsync(string jti);
    Task CleanupExpiredTokensAsync();
}