using ONEE.SSO.Application.Features.Users.Commands;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.Application.Features.Users.Handlers;

public class UnlockUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogService _auditLogService;

    public UnlockUserCommandHandler(
        IUserRepository userRepository,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _auditLogService = auditLogService;
    }

    public async Task<bool> HandleAsync(UnlockUserCommand command)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            return false;
        }

        if (!user.IsLocked)
        {
            // Déjà déverrouillé
            return true;
        }

        // Débloquer le compte
        user.IsLocked = false;
        user.LockedAt = null;
        user.FailedLoginAttempts = 0;
        user.LastFailedLoginAt = null;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        // Log déblocage
        await _auditLogService.LogAsync(
            command.AdminUserId,
            "AccountUnlocked",
            "User",
            user.Id,
            null,
            $"{{\"unlockedBy\": \"{command.AdminUserId}\", \"timestamp\": \"{DateTime.UtcNow:O}\"}}",
            command.IpAddress,
            null);

        return true;
    }
}