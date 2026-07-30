using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IUserSessionService
{
    Task<IEnumerable<UserSessionDto>> GetAllAsync();

    Task<UserSessionDto?> GetByIdAsync(Guid id);

    Task RevokeAsync(Guid id);
}