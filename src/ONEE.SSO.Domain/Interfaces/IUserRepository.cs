using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByEmailAsync(string email);

    Task<IEnumerable<User>> GetAllAsync();

    Task AddAsync(User user);

    Task UpdateAsync(User user);

    Task DeleteAsync(User user);

    // Gestion des rôles
    Task<IEnumerable<UserRole>> GetUserRolesAsync(Guid userId);
    
    Task AddUserRoleAsync(UserRole userRole);
    
    void RemoveUserRole(UserRole userRole);
}