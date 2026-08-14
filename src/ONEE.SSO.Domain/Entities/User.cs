using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Security - Account Lockout
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LastFailedLoginAt { get; set; }
    public bool IsLocked { get; set; } = false;
    public DateTime? LockedAt { get; set; }

    // Security - Password Reset
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiresAt { get; set; }

    // Security - Email Verification
    public bool IsEmailVerified { get; set; } = false;
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}