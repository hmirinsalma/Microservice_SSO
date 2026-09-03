using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;
using ONEE.SSO.Infrastructure.Persistence;

namespace ONEE.SSO.Infrastructure.Repositories;

public class UserConsentRepository : Repository<UserConsent>, IUserConsentRepository
{
    public UserConsentRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<UserConsent?> GetByUserAndClientAsync(Guid userId, string clientId)
    {
        return await _context.UserConsents
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ClientId == clientId);
    }

    public async Task<IEnumerable<UserConsent>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserConsents
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.GrantedAt)
            .ToListAsync();
    }

    public async Task<bool> HasValidConsentAsync(Guid userId, string clientId)
    {
        var consent = await GetByUserAndClientAsync(userId, clientId);
        
        if (consent == null)
            return false;
        
        // Vérifier si le consentement n'est pas expiré
        if (consent.ExpiresAt.HasValue && consent.ExpiresAt.Value < DateTime.UtcNow)
            return false;
        
        return true;
    }

    public async Task RevokeConsentAsync(Guid userId, string clientId)
    {
        var consents = await _context.UserConsents
            .Where(c => c.UserId == userId && c.ClientId == clientId)
            .ToListAsync();
        
        _context.UserConsents.RemoveRange(consents);
        await _context.SaveChangesAsync();
    }
}
