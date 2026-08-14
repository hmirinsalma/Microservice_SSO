using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IUserSessionService
{
    Task<IEnumerable<UserSessionDto>> GetAllAsync();

    Task<UserSessionDto?> GetByIdAsync(Guid id);

    Task RevokeAsync(Guid id);

    Task<UserSessionDto> CreateSessionAsync(Guid userId, string sessionId, string? device, string? browser, string? operatingSystem, string? ipAddress);

    Task<UserSessionDto?> GetBySessionIdAsync(string sessionId);

    Task<int> RevokeAllUserSessionsAsync(Guid userId, string? ipAddress);

    Task RevokeSessionByRefreshTokenAsync(string refreshToken, string? ipAddress);
}