using Microsoft.Extensions.Caching.Memory;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.Infrastructure.Services;

public class JwtBlocklistService : IJwtBlocklistService
{
    private readonly IMemoryCache _cache;
    private const string BLOCKLIST_KEY_PREFIX = "jwt_revoked_";

    public JwtBlocklistService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task RevokeTokenAsync(string jti, DateTime expiration)
    {
        var key = BLOCKLIST_KEY_PREFIX + jti;
        
        // Stocker le jti révoqué jusqu'à son expiration naturelle
        _cache.Set(key, true, expiration);
        
        return Task.CompletedTask;
    }

    public Task<bool> IsTokenRevokedAsync(string jti)
    {
        var key = BLOCKLIST_KEY_PREFIX + jti;
        var isRevoked = _cache.TryGetValue(key, out _);
        
        return Task.FromResult(isRevoked);
    }

    public Task CleanupExpiredTokensAsync()
    {
        // MemoryCache nettoie automatiquement les entrées expirées
        // Pas d'action nécessaire
        return Task.CompletedTask;
    }
}