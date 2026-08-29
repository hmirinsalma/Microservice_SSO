using System.Security.Claims;
using ONEE.EAMS.Application.DTOs.User;

namespace ONEE.EAMS.Application.Interfaces;

/// <summary>
/// Service de gestion des utilisateurs métier EAMS.
/// Ne gère PAS les mots de passe, tokens ou sessions — responsabilité du SSO.
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(Guid id);
    Task<UserDto> GetProfileAsync(ClaimsPrincipal user);
    Task<UserDto> UpdateProfileAsync(UpdateProfileRequest request, ClaimsPrincipal user);
    // ChangePasswordAsync supprimée — le changement de mot de passe est délégué au SSO
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request);
    Task ToggleActiveAsync(Guid id);
}
