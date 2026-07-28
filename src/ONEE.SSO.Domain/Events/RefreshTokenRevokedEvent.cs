using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Domain.Events;

public sealed class RefreshTokenRevokedEvent : IDomainEvent
{
    public RefreshToken RefreshToken { get; }

    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public RefreshTokenRevokedEvent(RefreshToken refreshToken)
    {
        RefreshToken = refreshToken;
    }
}