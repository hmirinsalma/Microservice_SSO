using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    private static PermissionDto MapToDto(Permission permission)
    {
        return new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Code = permission.Code,
            Description = permission.Description,
            ClientId = permission.ClientId
        };
    }

    public async Task<IEnumerable<PermissionDto>> GetAllAsync()
    {
        var permissions = await _permissionRepository.GetAllAsync();

        return permissions.Select(MapToDto);
    }

    public async Task<PermissionDto?> GetByIdAsync(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);

        return permission is null ? null : MapToDto(permission);
    }

    public async Task<PermissionDto> CreateAsync(CreatePermissionDto dto)
    {
        if (await _permissionRepository.PermissionExistsAsync(dto.Code, dto.ClientId))
            throw new InvalidOperationException(
                "A permission with this code already exists for this application.");

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description ?? string.Empty,
            ClientId = dto.ClientId,
            CreatedAt = DateTime.UtcNow
        };

        await _permissionRepository.AddAsync(permission);
        await _permissionRepository.SaveChangesAsync();

        return MapToDto(permission);
    }

    public async Task<PermissionDto> UpdateAsync(Guid id, UpdatePermissionDto dto)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);

        if (permission == null)
            throw new KeyNotFoundException("Permission not found.");

        permission.Name = dto.Name;
        permission.Code = dto.Code;
        permission.Description = dto.Description ?? string.Empty;
        permission.ClientId = dto.ClientId;
        permission.UpdatedAt = DateTime.UtcNow;

        _permissionRepository.Update(permission);
        await _permissionRepository.SaveChangesAsync();

        return MapToDto(permission);
    }
    public async Task<IEnumerable<PermissionDto>> GetByClientAsync(Guid clientId)
    {
        var permissions = await _permissionRepository.GetByClientAsync(clientId);

        return permissions.Select(MapToDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);

        if (permission is null)
            throw new KeyNotFoundException("Permission not found.");

        _permissionRepository.Delete(permission);
        await _permissionRepository.SaveChangesAsync();
    }
}