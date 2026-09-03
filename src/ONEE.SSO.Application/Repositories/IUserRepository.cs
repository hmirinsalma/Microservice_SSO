using ONEE.SSO.Application.Interfaces.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Application.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);

    Task<bool> EmailExistsAsync(string email);

    // Gestion des rôles utilisateur
    Task<IEnumerable<UserRole>> GetUserRolesAsync(Guid userId);
    
    Task AddUserRoleAsync(UserRole userRole);
    
    void RemoveUserRole(UserRole userRole);
}