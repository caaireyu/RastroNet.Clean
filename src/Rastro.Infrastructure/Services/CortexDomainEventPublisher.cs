using Cortex.Mediator;
using Microsoft.Extensions.Logging;
using Rastro.Application.Interfaces;
using Rastro.Domain.Primitives;

namespace Rastro.Infrastructure.Services;

public class CortexDomainEventPublisher : IDomainEventPublisher
{
    private readonly IMediator _mediator;
    private readonly ILogger<CortexDomainEventPublisher> _logger;

    public CortexDomainEventPublisher(IMediator mediator, ILogger<CortexDomainEventPublisher> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        _logger.LogInformation("Publicando Event de dominio: {EventType} con el ID: {EventId}", 
            typeof(T).Name, domainEvent.EventId);
        try
        {
            await _mediator.PublishAsync(domainEvent, cancellationToken);
            _logger.LogInformation("Publicación satisfactoria evento: {EventType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publicando evento: {EventType}", typeof(T).Name);
            throw;
        }
    }
}