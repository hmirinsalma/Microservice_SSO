using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<IEnumerable<RefreshTokenDto>> GetAllAsync();

    Task<RefreshTokenDto?> GetByIdAsync(Guid id);

    Task RevokeAsync(Guid id);

    Task<RefreshTokenDto> GenerateRefreshTokenAsync(Guid userId, string? ipAddress);

    Task<RefreshTokenDto?> GetByTokenAsync(string token);

    Task<int> RevokeAllUserTokensAsync(Guid userId, string? ipAddress);

    Task RevokeTokenAsync(string token, string? ipAddress, string? replacedByToken = null);

    Task<bool> ValidateRefreshTokenAsync(string token);
}