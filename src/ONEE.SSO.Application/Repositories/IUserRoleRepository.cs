using ONEE.SSO.Application.Interfaces.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Application.Repositories;

public interface IUserRoleRepository : IRepository<UserRole>
{
    Task<UserRole?> GetAsync(Guid userId, Guid roleId);

    Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId);

    Task<IEnumerable<UserRole>> GetByRoleIdAsync(Guid roleId);

    Task<bool> ExistsAsync(Guid userId, Guid roleId);
}