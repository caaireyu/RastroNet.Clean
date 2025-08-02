using Cortex.Mediator.Notifications;

namespace Rastro.Domain.Primitives;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}