using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;
using ONEE.SSO.Infrastructure.Persistence;

namespace ONEE.SSO.Infrastructure.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string name, Guid clientId)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r =>
                r.Name == name &&
                r.ClientId == clientId);
    }

    public async Task<bool> RoleExistsAsync(string name, Guid clientId)
    {
        return await _context.Roles
            .AnyAsync(r =>
                r.Name == name &&
                r.ClientId == clientId);
    }

    public async Task<IEnumerable<Role>> GetByClientAsync(Guid clientId)
    {
        return await _context.Roles
            .Where(r => r.ClientId == clientId)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }
}