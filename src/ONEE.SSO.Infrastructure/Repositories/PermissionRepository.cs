using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;
using ONEE.SSO.Infrastructure.Persistence;

namespace ONEE.SSO.Infrastructure.Repositories;

public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Permission?> GetByCodeAsync(string code, Guid clientId)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p =>
                p.Code == code &&
                p.ClientId == clientId);
    }

    public async Task<bool> PermissionExistsAsync(string code, Guid clientId)
    {
        return await _context.Permissions
            .AnyAsync(p =>
                p.Code == code &&
                p.ClientId == clientId);
    }

    public async Task<IEnumerable<Permission>> GetByClientAsync(Guid clientId)
    {
        return await _context.Permissions
            .Where(p => p.ClientId == clientId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}