using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IAuditLogService
{
    Task<IEnumerable<AuditLogDto>> GetAllAsync();

    Task<AuditLogDto?> GetByIdAsync(Guid id);

    Task LogAsync(
        Guid? userId,
        string action,
        string entityName,
        Guid? entityId,
        string? oldValues,
        string? newValues,
        string? ipAddress,
        string? userAgent);
}