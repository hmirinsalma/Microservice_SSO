using ONEE.SSO.Application.Interfaces.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Application.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, Guid clientId);

    Task<bool> RoleExistsAsync(string name, Guid clientId);

    Task<IEnumerable<Role>> GetByClientAsync(Guid clientId);
}