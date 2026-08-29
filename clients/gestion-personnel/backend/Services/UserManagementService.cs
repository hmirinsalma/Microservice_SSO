using GestionPersonnel.API.DTOs.User;
using GestionPersonnel.API.Exceptions;
using GestionPersonnel.API.Models;
using GestionPersonnel.API.Repositories.Interfaces;
using GestionPersonnel.API.Services.Interfaces;

namespace GestionPersonnel.API.Services;

/// <summary>
/// Service métier de gestion des comptes utilisateurs.
///
/// SSO-Ready :
///   - Aucune dépendance à BCrypt, StubCredentials ou à toute logique d'authentification.
///   - La création du credential temporaire est déléguée à IStubCredentialService
///     via le Controller (couche appelante), pas ici.
///   - Lors du passage au SSO, ce service ne changera pas.
/// </summary>
public class UserManagementService : IUserManagementService
{
    private readonly IUserManagementRepository _repo;

    public UserManagementService(IUserManagementRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
        => (await _repo.GetAllAsync()).Select(ToDto);

    public async Task<UserDto> GetByIdAsync(int id)
        => ToDto(await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Utilisateur", id));

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        => (await _repo.GetAllRolesAsync())
            .Select(r => new RoleDto { Id = r.Id, Nom = r.Nom });

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        if (await _repo.GetByEmailAsync(dto.Email) != null)
            throw new ConflictException($"L'email '{dto.Email}' est déjà utilisé.");

        if (await _repo.GetByUsernameAsync(dto.Username) != null)
            throw new ConflictException($"Le nom d'utilisateur '{dto.Username}' est déjà utilisé.");

        var user = new User
        {
            Username  = dto.Username.Trim(),
            Email     = dto.Email.Trim().ToLower(),
            RoleId    = dto.RoleId,
            IsActive  = true,
            CreatedAt = DateTime.UtcNow,
        };

        var created  = await _repo.CreateAsync(user);
        var reloaded = await _repo.GetByIdAsync(created.Id) ?? created;
        return ToDto(reloaded);
        // NOTE : la création du StubCredential est gérée par UsersController
        // via IStubCredentialService — aucune logique d'auth ici.
    }

    public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Utilisateur", id);

        var byEmail = await _repo.GetByEmailAsync(dto.Email);
        if (byEmail != null && byEmail.Id != id)
            throw new ConflictException($"L'email '{dto.Email}' est déjà utilisé.");

        var byUsername = await _repo.GetByUsernameAsync(dto.Username);
        if (byUsername != null && byUsername.Id != id)
            throw new ConflictException($"Le nom d'utilisateur '{dto.Username}' est déjà utilisé.");

        user.Username = dto.Username.Trim();
        user.Email    = dto.Email.Trim().ToLower();
        user.RoleId   = dto.RoleId;
        user.IsActive = dto.IsActive;

        var updated  = await _repo.UpdateAsync(user);
        var reloaded = await _repo.GetByIdAsync(updated.Id) ?? updated;
        return ToDto(reloaded);
    }

    public async Task ToggleActiveAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Utilisateur", id);
        user.IsActive = !user.IsActive;
        await _repo.UpdateAsync(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Utilisateur", id);
        await _repo.DeleteAsync(user);
        // NOTE : StubCredentialService.DeleteAsync est appelé par UsersController
        // via IStubCredentialService avant d'appeler cette méthode.
    }

    private static UserDto ToDto(User u) => new()
    {
        Id        = u.Id,
        Username  = u.Username,
        Email     = u.Email,
        Role      = u.Role?.Nom ?? string.Empty,
        IsActive  = u.IsActive,
        CreatedAt = u.CreatedAt,
        SsoId     = u.SsoId,
    };
}
