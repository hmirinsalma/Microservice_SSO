using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.Infrastructure.Services;

public class UserSessionService : IUserSessionService
{
    private readonly IUserSessionRepository _repository;

    public UserSessionService(IUserSessionRepository repository)
    {
        _repository = repository;
    }

    private static UserSessionDto MapToDto(Domain.Entities.UserSession session)
    {
        return new UserSessionDto
        {
            Id = session.Id,
            UserId = session.UserId,
            LoginAt = session.LoginAt,
            LogoutAt = session.LogoutAt,
            IsActive = session.IsActive
        };
    }

    public async Task<IEnumerable<UserSessionDto>> GetAllAsync()
    {
        var sessions = await _repository.GetAllAsync();
        return sessions.Select(MapToDto);
    }

    public async Task<UserSessionDto?> GetByIdAsync(Guid id)
    {
        var session = await _repository.GetByIdAsync(id);

        if (session == null)
            return null;

        return MapToDto(session);
    }

    public async Task RevokeAsync(Guid id)
    {
        var session = await _repository.GetByIdAsync(id);

        if (session == null)
            throw new Exception("Session not found.");

        session.IsActive = false;
        session.LogoutAt = DateTime.UtcNow;

        _repository.Update(session);

        await _repository.SaveChangesAsync();
    }
}