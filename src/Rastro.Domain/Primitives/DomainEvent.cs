namespace Rastro.Domain.Primitives;

public abstract class DomainEvent: IDomainEvent
{
    public Guid EventId { get; private set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;
}