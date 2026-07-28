namespace ONEE.SSO.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}