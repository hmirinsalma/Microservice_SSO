using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<IEnumerable<RefreshTokenDto>> GetAllAsync();

    Task<RefreshTokenDto?> GetByIdAsync(Guid id);

    Task RevokeAsync(Guid id);
}