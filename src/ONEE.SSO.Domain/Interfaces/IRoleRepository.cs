using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);

    Task<Role?> GetByNameAsync(string name);

    Task<IEnumerable<Role>> GetAllAsync();

    Task AddAsync(Role role);

    Task UpdateAsync(Role role);

    Task DeleteAsync(Role role);
}