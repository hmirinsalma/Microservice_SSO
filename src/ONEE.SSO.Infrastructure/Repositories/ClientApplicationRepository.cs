using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;
using ONEE.SSO.Infrastructure.Persistence;

namespace ONEE.SSO.Infrastructure.Repositories;

public class ClientApplicationRepository : Repository<ClientApplication>, IClientApplicationRepository
{
    public ClientApplicationRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<ClientApplication?> GetByClientIdAsync(string clientId)
    {
        return await _context.ClientApplications
            .FirstOrDefaultAsync(c => c.ClientId == clientId);
    }

    public async Task<bool> ClientExistsAsync(string clientId)
    {
        return await _context.ClientApplications
            .AnyAsync(c => c.ClientId == clientId);
    }
    public async Task<IEnumerable<ClientApplication>> SearchAsync(string keyword)
    {
        return await _context.ClientApplications
            .Where(c =>
                c.Name.Contains(keyword) ||
                c.ClientId.Contains(keyword))
            .ToListAsync();
    }
    public async Task<IEnumerable<ClientApplication>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _context.ClientApplications
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}