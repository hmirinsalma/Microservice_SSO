using GestionPersonnel.API.DTOs.User;

namespace GestionPersonnel.API.Services.Interfaces;

/// <summary>
/// Gestion métier des comptes utilisateurs.
/// N'inclut aucune opération liée aux mots de passe (délégué au SSO).
/// </summary>
public interface IUserManagementService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(int id);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto> UpdateAsync(int id, UpdateUserDto dto);
    Task ToggleActiveAsync(int id);
    Task DeleteAsync(int id);
}
