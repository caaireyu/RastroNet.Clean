using Rastro.Domain.Primitives;

namespace Rastro.Application.Interfaces;

public interface IDomainEventPublisher
{
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default)
        where T : IDomainEvent;
}