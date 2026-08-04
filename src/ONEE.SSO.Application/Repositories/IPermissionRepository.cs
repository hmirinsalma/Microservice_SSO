using ONEE.SSO.Application.Interfaces.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Application.Repositories;

public interface IPermissionRepository : IRepository<Permission>
{
    Task<Permission?> GetByCodeAsync(string code, Guid clientId);

    Task<bool> PermissionExistsAsync(string code, Guid clientId);

    Task<IEnumerable<Permission>> GetByClientAsync(Guid clientId);
}