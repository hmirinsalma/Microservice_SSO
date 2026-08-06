using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Services;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserRoleService(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var role = await _roleRepository.GetByIdAsync(roleId);

        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        if (await _userRoleRepository.ExistsAsync(userId, roleId))
            throw new InvalidOperationException("User already has this role.");

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow
        };

        await _userRoleRepository.AddAsync(userRole);
        await _userRoleRepository.SaveChangesAsync();
    }

    public async Task RemoveRoleAsync(Guid userId, Guid roleId)
    {
        var userRole = await _userRoleRepository.GetAsync(userId, roleId);

        if (userRole == null)
            throw new KeyNotFoundException("User role not found.");

        _userRoleRepository.Delete(userRole);
        await _userRoleRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<RoleDto>> GetRolesByUserAsync(Guid userId)
    {
        var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);

        return userRoles.Select(ur => new RoleDto
        {
            Id = ur.Role.Id,
            Name = ur.Role.Name,
            Description = ur.Role.Description,
            IsSystemRole = ur.Role.IsSystemRole,
            ClientId = ur.Role.ClientId
        });
    }

    public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(Guid roleId)
    {
        var userRoles = await _userRoleRepository.GetByRoleIdAsync(roleId);

        return userRoles.Select(ur => new UserDto
        {
            Id = ur.User.Id,
            FirstName = ur.User.FirstName,
            LastName = ur.User.LastName,
            Email = ur.User.Email,
            IsActive = ur.User.IsActive
        });
    }
}