using ONEE.SSO.Application.Features.Auth.Commands;
using ONEE.SSO.Application.Features.Auth.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.Application.Features.Auth.Handlers;

public class ChangePasswordCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordValidationService _passwordValidationService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserSessionService _userSessionService;
    private readonly IJwtService _jwtService;
    private readonly IAuditLogService _auditLogService;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IPasswordValidationService passwordValidationService,
        IRefreshTokenService refreshTokenService,
        IUserSessionService userSessionService,
        IJwtService jwtService,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _passwordValidationService = passwordValidationService;
        _refreshTokenService = refreshTokenService;
        _userSessionService = userSessionService;
        _jwtService = jwtService;
        _auditLogService = auditLogService;
    }

    public async Task<PasswordOperationResponseDto> HandleAsync(ChangePasswordCommand command)
    {
        // Récupérer l'utilisateur
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null || !user.IsActive)
        {
            return new PasswordOperationResponseDto
            {
                Success = false,
                Message = "Utilisateur introuvable ou inactif."
            };
        }

        // Vérifier l'ancien mot de passe
        if (!_passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
        {
            return new PasswordOperationResponseDto
            {
                Success = false,
                Message = "Mot de passe actuel incorrect."
            };
        }

        // Valider le nouveau mot de passe
        var (isValid, errorMessage) = _passwordValidationService.ValidatePassword(command.NewPassword);
        if (!isValid)
        {
            return new PasswordOperationResponseDto
            {
                Success = false,
                Message = errorMessage ?? "Mot de passe invalide."
            };
        }

        // Vérifier que le nouveau mot de passe est différent
        if (command.CurrentPassword == command.NewPassword)
        {
            return new PasswordOperationResponseDto
            {
                Success = false,
                Message = "Le nouveau mot de passe doit être différent de l'actuel."
            };
        }

        // Mettre à jour le mot de passe
        user.PasswordHash = _passwordHasher.Hash(command.NewPassword);
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        // Révoquer toutes les sessions SAUF la session courante
        if (!string.IsNullOrEmpty(command.AccessToken))
        {
            var currentJti = _jwtService.GetJtiFromToken(command.AccessToken);
            
            // Révoquer tous les refresh tokens
            await _refreshTokenService.RevokeAllUserTokensAsync(user.Id, command.IpAddress);
            
            // Invalider toutes les sessions
            await _userSessionService.RevokeAllUserSessionsAsync(user.Id, command.IpAddress);
        }
        else
        {
            // Si pas de token fourni, révoquer tout
            await _refreshTokenService.RevokeAllUserTokensAsync(user.Id, command.IpAddress);
            await _userSessionService.RevokeAllUserSessionsAsync(user.Id, command.IpAddress);
        }

        // Log changement de mot de passe
        await _auditLogService.LogAsync(
            user.Id,
            "PasswordChanged",
            "User",
            user.Id,
            null,
            null,
            command.IpAddress,
            null);

        return new PasswordOperationResponseDto
        {
            Success = true,
            Message = "Mot de passe changé avec succès. Les autres sessions ont été révoquées."
        };
    }
}