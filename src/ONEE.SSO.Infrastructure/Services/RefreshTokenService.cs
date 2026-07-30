using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;

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
}