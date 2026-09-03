using ONEE.SSO.Domain.Common;

namespace ONEE.SSO.Domain.Entities;

public class Notification : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    
    public User? User { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Message { get; set; } = string.Empty;
    
    public string Type { get; set; } = "info"; // info, success, warning, error
    
    public bool IsRead { get; set; } = false;
    
    public string? ClientApplicationName { get; set; }
    
    public string? IpAddress { get; set; }
    
    public string? UserAgent { get; set; }
}
