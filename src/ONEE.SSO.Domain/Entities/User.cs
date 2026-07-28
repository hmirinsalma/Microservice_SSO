using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}