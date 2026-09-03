using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Services;

public class UserSessionService : IUserSessionService
{
    private readonly IUserSessionRepository _repository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;

    public UserSessionService(
        IUserSessionRepository repository, 
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
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

    public async Task<UserSessionDto> CreateSessionAsync(Guid userId, string sessionId, string? device, string? browser, string? operatingSystem, string? ipAddress)
    {
        var session = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SessionId = sessionId,
            Device = device,
            Browser = browser,
            OperatingSystem = operatingSystem,
            IpAddress = ipAddress,
            LoginAt = DateTime.UtcNow,
            IsActive = true
        };

        await _repository.AddAsync(session);
        await _repository.SaveChangesAsync();

        return MapToDto(session);
    }

    public async Task<UserSessionDto?> GetBySessionIdAsync(string sessionId)
    {
        var sessions = await _repository.GetAllAsync();
        var session = sessions.FirstOrDefault(s => s.SessionId == sessionId);

        if (session == null)
            return null;

        return MapToDto(session);
    }

    public async Task<int> RevokeAllUserSessionsAsync(Guid userId, string? ipAddress)
    {
        var sessions = await _repository.GetAllAsync();
        var userActiveSessions = sessions.Where(s => s.UserId == userId && s.IsActive).ToList();

        foreach (var session in userActiveSessions)
        {
            session.IsActive = false;
            session.LogoutAt = DateTime.UtcNow;
            _repository.Update(session);
        }

        if (userActiveSessions.Any())
        {
            await _repository.SaveChangesAsync();
        }

        return userActiveSessions.Count;
    }

    public async Task RevokeSessionByRefreshTokenAsync(string refreshToken, string? ipAddress)
    {
        // Récupérer le refresh token pour obtenir l'UserId
        var refreshTokens = await _refreshTokenRepository.GetAllAsync();
        var tokenEntity = refreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

        if (tokenEntity == null)
            return;

        // Révoquer la session la plus récente active de cet utilisateur
        var sessions = await _repository.GetAllAsync();
        var activeSession = sessions
            .Where(s => s.UserId == tokenEntity.UserId && s.IsActive)
            .OrderByDescending(s => s.LoginAt)
            .FirstOrDefault();

        if (activeSession != null)
        {
            activeSession.IsActive = false;
            activeSession.LogoutAt = DateTime.UtcNow;
            _repository.Update(activeSession);
            await _repository.SaveChangesAsync();
        }
    }

    public async Task<bool> HasActiveSessionAsync(string email)
    {
        // Récupérer l'utilisateur par email
        var user = await _userRepository.GetByEmailAsync(email);
        
        if (user == null)
            return false;

        // Vérifier s'il a une session active
        var sessions = await _repository.GetAllAsync();
        var hasActiveSession = sessions.Any(s => s.UserId == user.Id && s.IsActive);

        return hasActiveSession;
    }
}