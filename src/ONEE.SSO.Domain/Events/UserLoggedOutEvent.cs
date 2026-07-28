using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Domain.Events;

public sealed class UserLoggedOutEvent : IDomainEvent
{
    public User User { get; }

    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserLoggedOutEvent(User user)
    {
        User = user;
    }
}