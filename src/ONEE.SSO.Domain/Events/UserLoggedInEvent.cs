using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Domain.Events;

public sealed class UserLoggedInEvent : IDomainEvent
{
    public User User { get; }

    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserLoggedInEvent(User user)
    {
        User = user;
    }
}