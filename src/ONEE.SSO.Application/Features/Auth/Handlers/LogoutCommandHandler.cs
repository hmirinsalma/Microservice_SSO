using ONEE.SSO.Application.Features.Auth.Commands;
using ONEE.SSO.Application.Features.Auth.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.Application.Features.Auth.Handlers;

public class LogoutCommandHandler
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserSessionService _userSessionService;
    private readonly IAuditLogService _auditLogService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(
        IRefreshTokenService refreshTokenService,
        IUserSessionService userSessionService,
        IAuditLogService auditLogService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenService = refreshTokenService;
        _userSessionService = userSessionService;
        _auditLogService = auditLogService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<LogoutResponseDto> HandleAsync(LogoutCommand command)
    {
        try
        {
            if (command.LogoutAllDevices)
            {
                return await HandleLogoutAllDevicesAsync(command);
            }

            return await HandleSingleLogoutAsync(command);
        }
        catch
        {
            return new LogoutResponseDto
            {
                Success = false,
                Message = "Erreur lors de la déconnexion",
                SessionsRevoked = 0
            };
        }
    }

    private async Task<LogoutResponseDto> HandleSingleLogoutAsync(LogoutCommand command)
    {
        if (string.IsNullOrEmpty(command.RefreshToken))
        {
            return new LogoutResponseDto
            {
                Success = false,
                Message = "RefreshToken requis",
                SessionsRevoked = 0
            };
        }

        var refreshToken = await _refreshTokenService.GetByTokenAsync(command.RefreshToken);
        if (refreshToken == null)
        {
            return new LogoutResponseDto
            {
                Success = false,
                Message = "RefreshToken invalide",
                SessionsRevoked = 0
            };
        }

        // Révoquer le refresh token
        await _refreshTokenService.RevokeTokenAsync(command.RefreshToken, command.IpAddress);

        // Invalider la session correspondante
        await _userSessionService.RevokeSessionByRefreshTokenAsync(command.RefreshToken, command.IpAddress);

        // Enregistrer l'audit log
        await _auditLogService.LogAsync(
            refreshToken.UserId,
            "Logout",
            "User",
            refreshToken.UserId,
            null,
            null,
            command.IpAddress,
            null);

        return new LogoutResponseDto
        {
            Success = true,
            Message = "Déconnexion réussie",
            SessionsRevoked = 1
        };
    }

    private async Task<LogoutResponseDto> HandleLogoutAllDevicesAsync(LogoutCommand command)
    {
        if (string.IsNullOrEmpty(command.RefreshToken))
        {
            return new LogoutResponseDto
            {
                Success = false,
                Message = "RefreshToken requis pour identifier l'utilisateur",
                SessionsRevoked = 0
            };
        }

        var refreshToken = await _refreshTokenService.GetByTokenAsync(command.RefreshToken);
        if (refreshToken == null)
        {
            return new LogoutResponseDto
            {
                Success = false,
                Message = "RefreshToken invalide",
                SessionsRevoked = 0
            };
        }

        // Révoquer tous les refresh tokens de l'utilisateur
        var tokensRevoked = await _refreshTokenService.RevokeAllUserTokensAsync(refreshToken.UserId, command.IpAddress);

        // Invalider toutes les sessions de l'utilisateur
        var sessionsRevoked = await _userSessionService.RevokeAllUserSessionsAsync(refreshToken.UserId, command.IpAddress);

        // Enregistrer l'audit log
        await _auditLogService.LogAsync(
            refreshToken.UserId,
            "LogoutAllDevices",
            "User",
            refreshToken.UserId,
            null,
            null,
            command.IpAddress,
            null);

        return new LogoutResponseDto
        {
            Success = true,
            Message = $"Déconnexion globale réussie. {sessionsRevoked} sessions révoquées.",
            SessionsRevoked = sessionsRevoked
        };
    }
}