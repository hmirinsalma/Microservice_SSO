using ONEE.SSO.Application.Interfaces.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Application.Repositories;

public interface IUserSessionRepository : IRepository<UserSession>
{
    Task<IEnumerable<UserSession>> GetActiveSessionsAsync(Guid userId);

    Task<UserSession?> GetBySessionIdAsync(string sessionId);
}