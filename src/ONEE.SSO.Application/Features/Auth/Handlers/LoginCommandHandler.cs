using ONEE.SSO.Application.Features.Auth.Commands;
using ONEE.SSO.Application.Features.Auth.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.Application.Features.Auth.Handlers;

public class LoginCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserSessionService _userSessionService;
    private readonly IAuditLogService _auditLogService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUserRoleRepository userRoleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        IUserSessionService userSessionService,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _userSessionService = userSessionService;
        _auditLogService = auditLogService;
    }

    public async Task<LoginResponseDto?> HandleAsync(LoginCommand command)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email);

        if (user == null)
        {
            // Log failed login attempt
            await _auditLogService.LogAsync(
                null,
                "LoginFailed",
                "User",
                null,
                null,
                null,
                command.IpAddress,
                command.UserAgent);
            
            return null;
        }

        if (!user.IsActive)
        {
            // Log inactive user login attempt
            await _auditLogService.LogAsync(
                user.Id,
                "LoginFailed",
                "User",
                user.Id,
                null,
                null,
                command.IpAddress,
                command.UserAgent);
            
            return null;
        }

        var passwordValid = _passwordHasher.Verify(
            command.Password,
            user.PasswordHash);

        if (!passwordValid)
        {
            // Log invalid password attempt
            await _auditLogService.LogAsync(
                user.Id,
                "LoginFailed",
                "User",
                user.Id,
                null,
                null,
                command.IpAddress,
                command.UserAgent);
            
            return null;
        }

        // Récupérer les rôles de l'utilisateur
        var userRoles = await _userRoleRepository.GetByUserIdAsync(user.Id);

        var roles = userRoles
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        // Récupérer les permissions de tous les rôles
        var permissions = new List<string>();

        foreach (var userRole in userRoles)
        {
            var rolePermissions =
                await _rolePermissionRepository
                    .GetByRoleIdAsync(userRole.RoleId);

            permissions.AddRange(
                rolePermissions.Select(rp => rp.Permission.Code));
        }

        permissions = permissions
            .Distinct()
            .ToList();

        // Générer le JWT Access Token
        var accessToken = _jwtService.GenerateAccessToken(
            user.Id,
            user.Email,
            roles,
            permissions);

        // Générer le Refresh Token
        var refreshTokenDto = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, command.IpAddress);

        // Créer une session utilisateur
        var sessionId = Guid.NewGuid().ToString();
        var userAgent = command.UserAgent ?? "";
        var device = ExtractDeviceInfo(userAgent);
        var browser = ExtractBrowserInfo(userAgent);
        var os = ExtractOSInfo(userAgent);

        await _userSessionService.CreateSessionAsync(
            user.Id,
            sessionId,
            device,
            browser,
            os,
            command.IpAddress);

        // Log successful login
        await _auditLogService.LogAsync(
            user.Id,
            "Login",
            "User",
            user.Id,
            null,
            null,
            command.IpAddress,
            command.UserAgent);

        return new LoginResponseDto
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshTokenDto.Token,
            RefreshTokenExpiresAt = refreshTokenDto.ExpiresAt,
            Roles = roles
        };
    }

    private static string ExtractDeviceInfo(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return "Unknown Device";

        if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
            return "Mobile";
        
        if (userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
            return "Tablet";
            
        return "Desktop";
    }

    private static string ExtractBrowserInfo(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return "Unknown Browser";

        if (userAgent.Contains("Chrome"))
            return "Chrome";
        
        if (userAgent.Contains("Firefox"))
            return "Firefox";
        
        if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
            return "Safari";
        
        if (userAgent.Contains("Edge"))
            return "Edge";
            
        return "Other";
    }

    private static string ExtractOSInfo(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return "Unknown OS";

        if (userAgent.Contains("Windows"))
            return "Windows";
        
        if (userAgent.Contains("Mac"))
            return "macOS";
        
        if (userAgent.Contains("Linux"))
            return "Linux";
        
        if (userAgent.Contains("Android"))
            return "Android";
        
        if (userAgent.Contains("iOS"))
            return "iOS";
            
        return "Other";
    }
}