using ONEE.SSO.Application.Features.Auth.Commands;
using ONEE.SSO.Application.Features.Auth.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.Application.Features.Auth.Handlers;

public class ResetPasswordCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordValidationService _passwordValidationService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserSessionService _userSessionService;
    private readonly IAuditLogService _auditLogService;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IPasswordValidationService passwordValidationService,
        IRefreshTokenService refreshTokenService,
        IUserSessionService userSessionService,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _passwordValidationService = passwordValidationService;
        _refreshTokenService = refreshTokenService;
        _userSessionService = userSessionService;
        _auditLogService = auditLogService;
    }

    public async Task<PasswordOperationResponseDto> HandleAsync(ResetPasswordCommand command)
    {
        // Valider le format du nouveau mot de passe
        var (isValid, errorMessage) = _passwordValidationService.ValidatePassword(command.NewPassword);
        if (!isValid)
        {
            return new PasswordOperationResponseDto
            {
                Success = false,
                Message = errorMessage ?? "Mot de passe invalide."
            };
        }

        // Trouver l'utilisateur avec ce token
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.PasswordResetToken == command.Token);

        if (user == null)
        {
            return new PasswordOperationResponseDto
            {
                Success = false,
                Message = "Token de réinitialisation invalide ou expiré."
            };
        }

        // Vérifier l'expiration du token
        if (user.PasswordResetTokenExpiresAt == null || user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
        {
            return new PasswordOperationResponseDto
            {
                Success = false,
                Message = "Token de réinitialisation expiré."
            };
        }

        // Vérifier que le nouveau mot de passe est différent de l'actuel
        if (_passwordHasher.Verify(command.NewPassword, user.PasswordHash))
        {
            return new PasswordOperationResponseDto
            {
                Success = false,
                Message = "Le nouveau mot de passe doit être différent de l'actuel."
            };
        }

        // Mettre à jour le mot de passe
        user.PasswordHash = _passwordHasher.Hash(command.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;

        // Révoquer tous les refresh tokens et sessions
        await _refreshTokenService.RevokeAllUserTokensAsync(user.Id, command.IpAddress);
        await _userSessionService.RevokeAllUserSessionsAsync(user.Id, command.IpAddress);

        // Réinitialiser le compteur de tentatives échouées si verrouillé
        if (user.IsLocked)
        {
            user.IsLocked = false;
            user.LockedAt = null;
            user.FailedLoginAttempts = 0;
            user.LastFailedLoginAt = null;
        }

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        // Log réinitialisation réussie
        await _auditLogService.LogAsync(
            user.Id,
            "PasswordReset",
            "User",
            user.Id,
            null,
            null,
            command.IpAddress,
            null);

        return new PasswordOperationResponseDto
        {
            Success = true,
            Message = "Mot de passe réinitialisé avec succès. Toutes les sessions ont été révoquées."
        };
    }
}