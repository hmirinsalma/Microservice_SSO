using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive
        };
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return null;

        return MapToDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return null;

        return MapToDto(user);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _userRepository.EmailExistsAsync(email);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        // Vérifier que l'email n'existe pas déjà
        if (await _userRepository.EmailExistsAsync(dto.Email))
        {
            throw new Exception($"Un utilisateur avec l'email {dto.Email} existe déjà.");
        }

        // Vérifier que les rôles existent
        var roles = new List<Role>();
        if (dto.RoleIds != null && dto.RoleIds.Any())
        {
            foreach (var roleId in dto.RoleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    throw new Exception($"Le rôle avec l'ID {roleId} n'existe pas.");
                }
                roles.Add(role);
            }
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UserRoles = roles.Select(r => new UserRole
            {
                RoleId = r.Id,
                UserId = Guid.NewGuid() // Sera remplacé par l'ID du user
            }).ToList()
        };

        // Corriger les UserId après création
        foreach (var userRole in user.UserRoles)
        {
            userRole.UserId = user.Id;
        }

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        // Mise à jour du mot de passe si fourni
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.PasswordHash = _passwordHasher.Hash(dto.Password);
        }

        // Mise à jour des rôles si fournis
        if (dto.RoleIds != null && dto.RoleIds.Any())
        {
            // Charger les UserRoles existants depuis la base
            var existingUserRoles = await _userRepository.GetUserRolesAsync(id);
            
            // Supprimer les anciens rôles
            foreach (var userRole in existingUserRoles)
            {
                _userRepository.RemoveUserRole(userRole);
            }
            
            // Sauvegarder pour appliquer les suppressions
            await _userRepository.SaveChangesAsync();

            // Ajouter les nouveaux rôles
            foreach (var roleId in dto.RoleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    throw new Exception($"Le rôle avec l'ID {roleId} n'existe pas.");
                }

                var newUserRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                };
                
                await _userRepository.AddUserRoleAsync(newUserRole);
            }
        }

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task ActivateAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }
    public async Task<IEnumerable<UserDto>> SearchAsync(string keyword)
    {
        var users = await _userRepository.FindAsync(u =>
            u.FirstName.Contains(keyword) ||
            u.LastName.Contains(keyword) ||
            u.Email.Contains(keyword));

        return users.Select(MapToDto);
    }

    public async Task<IEnumerable<UserDto>> GetPagedAsync(int page, int pageSize)
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
           .Select(MapToDto);
    }
    public async Task<IEnumerable<UserDto>> FilterAsync(
    string? firstName,
    string? lastName,
    bool? isActive)
    {
        var users = await _userRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            users = users.Where(u =>
                u.FirstName.Contains(firstName,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            users = users.Where(u =>
                u.LastName.Contains(lastName,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (isActive.HasValue)
        {
            users = users.Where(u => u.IsActive == isActive.Value);
        }

        return users.Select(MapToDto);
    }
}