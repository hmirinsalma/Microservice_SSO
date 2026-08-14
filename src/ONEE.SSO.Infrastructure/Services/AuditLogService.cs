using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    private static AuditLogDto MapToDto(Domain.Entities.AuditLog log)
    {
        return new AuditLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            Action = log.Action,
            EntityName = log.EntityName,
            EntityId = log.EntityId,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent,
            CreatedAt = log.CreatedAt
        };
    }

    public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
    {
        var logs = await _repository.GetAllAsync();
        return logs.Select(MapToDto);
    }

    public async Task<AuditLogDto?> GetByIdAsync(Guid id)
    {
        var log = await _repository.GetByIdAsync(id);

        if (log == null)
            return null;

        return MapToDto(log);
    }

    public async Task LogAsync(
        Guid? userId,
        string action,
        string entityName,
        Guid? entityId,
        string? oldValues,
        string? newValues,
        string? ipAddress,
        string? userAgent)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.Empty, // Si null, utiliser Guid.Empty pour les actions anonymes
            Action = action,
            EntityName = entityName,
            EntityId = entityId?.ToString(),
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(auditLog);
        await _repository.SaveChangesAsync();
    }
}
