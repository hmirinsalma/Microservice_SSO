using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;
using ONEE.SSO.Infrastructure.Persistence;

namespace ONEE.SSO.Infrastructure.Repositories;

public class UserSessionRepository : Repository<UserSession>, IUserSessionRepository
{
    public UserSessionRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<UserSession>> GetActiveSessionsAsync(Guid userId)
    {
        return await _context.UserSessions
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync();
    }

    public async Task<UserSession?> GetBySessionIdAsync(string sessionId)
    {
        return await _context.UserSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);
    }
}