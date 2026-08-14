using System.Security.Cryptography;
using ONEE.SSO.Application.Features.Auth.Commands;
using ONEE.SSO.Application.Features.Auth.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.Application.Features.Auth.Handlers;

public class ForgotPasswordCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogService _auditLogService;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _auditLogService = auditLogService;
    }

    public async Task<PasswordOperationResponseDto> HandleAsync(ForgotPasswordCommand command)
    {
        // Toujours retourner succès pour éviter l'énumération des emails
        var genericResponse = new PasswordOperationResponseDto
        {
            Success = true,
            Message = "Si cet email existe, un lien de réinitialisation vous a été envoyé."
        };

        var user = await _userRepository.GetByEmailAsync(command.Email);
        if (user == null)
        {
            // Log tentative sur email inexistant
            await _auditLogService.LogAsync(
                null,
                "ForgotPasswordAttempt",
                "User",
                null,
                null,
                null,
                command.IpAddress,
                null);

            return genericResponse;
        }

        // Générer un token de réinitialisation sécurisé
        var resetToken = GenerateSecureToken();
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1); // 1 heure

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        // Log demande de réinitialisation
        await _auditLogService.LogAsync(
            user.Id,
            "ForgotPasswordRequested",
            "User",
            user.Id,
            null,
            null,
            command.IpAddress,
            null);

        // TODO: Envoyer l'email avec le token (implémenter IEmailService)
        // await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);

        return genericResponse;
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[32]; // 256 bits
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}