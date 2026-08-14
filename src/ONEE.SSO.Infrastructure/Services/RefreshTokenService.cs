using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;
using System.Security.Cryptography;

namespace ONEE.SSO.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _repository;

    public RefreshTokenService(IRefreshTokenRepository repository)
    {
        _repository = repository;
    }

    private static RefreshTokenDto MapToDto(Domain.Entities.RefreshToken token)
    {
        return new RefreshTokenDto
        {
            Id = token.Id,
            UserId = token.UserId,
            Token = token.Token,
            ExpiresAt = token.ExpiresAt,
            IsRevoked = token.IsRevoked
        };
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetAllAsync()
    {
        var tokens = await _repository.GetAllAsync();
        return tokens.Select(MapToDto);
    }

    public async Task<RefreshTokenDto?> GetByIdAsync(Guid id)
    {
        var token = await _repository.GetByIdAsync(id);

        if (token == null)
            return null;

        return MapToDto(token);
    }

    public async Task RevokeAsync(Guid id)
    {
        var token = await _repository.GetByIdAsync(id);

        if (token == null)
            throw new Exception("Refresh token not found.");

        token.RevokedAt = DateTime.UtcNow;

        _repository.Update(token);

        await _repository.SaveChangesAsync();
    }

    public async Task<RefreshTokenDto> GenerateRefreshTokenAsync(Guid userId, string? ipAddress)
    {
        var tokenValue = GenerateSecureToken();
        
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = tokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(30), // 30 jours
            CreatedByIp = ipAddress
        };

        await _repository.AddAsync(refreshToken);
        await _repository.SaveChangesAsync();

        return MapToDto(refreshToken);
    }

    public async Task<RefreshTokenDto?> GetByTokenAsync(string token)
    {
        var tokens = await _repository.GetAllAsync();
        var refreshToken = tokens.FirstOrDefault(rt => rt.Token == token);

        if (refreshToken == null)
            return null;

        return MapToDto(refreshToken);
    }

    public async Task<int> RevokeAllUserTokensAsync(Guid userId, string? ipAddress)
    {
        var tokens = await _repository.GetAllAsync();
        var userActiveTokens = tokens.Where(rt => rt.UserId == userId && rt.IsActive).ToList();

        foreach (var token in userActiveTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            _repository.Update(token);
        }

        if (userActiveTokens.Any())
        {
            await _repository.SaveChangesAsync();
        }

        return userActiveTokens.Count;
    }

    public async Task RevokeTokenAsync(string token, string? ipAddress, string? replacedByToken = null)
    {
        var tokens = await _repository.GetAllAsync();
        var refreshToken = tokens.FirstOrDefault(rt => rt.Token == token);

        if (refreshToken == null)
            return;

        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReplacedByToken = replacedByToken;

        _repository.Update(refreshToken);
        await _repository.SaveChangesAsync();
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token)
    {
        var refreshToken = await GetByTokenAsync(token);
        
        if (refreshToken == null)
            return false;

        var tokens = await _repository.GetAllAsync();
        var tokenEntity = tokens.FirstOrDefault(rt => rt.Token == token);

        return tokenEntity != null && tokenEntity.IsActive;
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[64]; // 512 bits
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}