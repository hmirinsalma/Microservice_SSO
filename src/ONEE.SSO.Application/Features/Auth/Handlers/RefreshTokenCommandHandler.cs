using ONEE.SSO.Application.Features.Auth.Commands;
using ONEE.SSO.Application.Features.Auth.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.Application.Features.Auth.Handlers;

public class RefreshTokenCommandHandler
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IJwtService _jwtService;
    private readonly IAuditLogService _auditLogService;

    public RefreshTokenCommandHandler(
        IRefreshTokenService refreshTokenService,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IJwtService jwtService,
        IAuditLogService auditLogService)
    {
        _refreshTokenService = refreshTokenService;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _jwtService = jwtService;
        _auditLogService = auditLogService;
    }

    public async Task<RefreshTokenResponseDto?> HandleAsync(RefreshTokenCommand command)
    {
        // Valider le refresh token
        var isValid = await _refreshTokenService.ValidateRefreshTokenAsync(command.RefreshToken);
        if (!isValid)
        {
            return null;
        }

        // Récupérer le refresh token
        var refreshTokenDto = await _refreshTokenService.GetByTokenAsync(command.RefreshToken);
        if (refreshTokenDto == null)
        {
            return null;
        }

        // Récupérer l'utilisateur
        var user = await _userRepository.GetByIdAsync(refreshTokenDto.UserId);
        if (user == null || !user.IsActive)
        {
            return null;
        }

        // Révoquer l'ancien refresh token et créer un nouveau
        var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, command.IpAddress);
        await _refreshTokenService.RevokeTokenAsync(command.RefreshToken, command.IpAddress, newRefreshToken.Token);

        // Récupérer les rôles et permissions pour le nouveau JWT
        var userRoles = await _userRoleRepository.GetByUserIdAsync(user.Id);
        var roles = userRoles.Select(ur => ur.Role.Name).Distinct().ToList();

        var permissions = new List<string>();
        foreach (var userRole in userRoles)
        {
            var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(userRole.RoleId);
            permissions.AddRange(rolePermissions.Select(rp => rp.Permission.Code));
        }
        permissions = permissions.Distinct().ToList();

        // Générer le nouveau access token
        var accessToken = _jwtService.GenerateAccessToken(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            roles,
            permissions);

        // Log de la rotation
        await _auditLogService.LogAsync(
            user.Id,
            "RefreshToken",
            "RefreshToken",
            refreshTokenDto.Id,
            null,
            null,
            command.IpAddress,
            null);

        return new RefreshTokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15) // Selon le spec : 15 minutes
        };
    }
}