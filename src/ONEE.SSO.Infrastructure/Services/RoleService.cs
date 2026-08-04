using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            ClientId = role.ClientId
        };
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();

        return roles.Select(MapToDto);
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role == null)
            return null;

        return MapToDto(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
    {
        if (await _roleRepository.RoleExistsAsync(dto.Name, dto.ClientId))
            throw new InvalidOperationException(
                "A role with this name already exists for this application.");

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description ?? string.Empty,
            IsSystemRole = dto.IsSystemRole,
            ClientId = dto.ClientId,
            CreatedAt = DateTime.UtcNow
        };

        await _roleRepository.AddAsync(role);
        await _roleRepository.SaveChangesAsync();

        return MapToDto(role);
    }

    public async Task<RoleDto> UpdateAsync(Guid id, UpdateRoleDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        role.Name = dto.Name;
        role.Description = dto.Description ?? string.Empty;
        role.IsSystemRole = dto.IsSystemRole;
        role.ClientId = dto.ClientId;
        role.UpdatedAt = DateTime.UtcNow;

        _roleRepository.Update(role);
        await _roleRepository.SaveChangesAsync();

        return MapToDto(role);
    }
    public async Task<IEnumerable<RoleDto>> GetByClientAsync(Guid clientId)
    {
        var roles = await _roleRepository.GetByClientAsync(clientId);

        return roles.Select(MapToDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        _roleRepository.Delete(role);
        await _roleRepository.SaveChangesAsync();
    }
}