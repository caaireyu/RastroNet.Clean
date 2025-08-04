using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Rastro.Application.Interfaces;
using Rastro.Domain.Primitives;
using Rastro.Domain.Users;

namespace Rastro.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IDomainEventPublisher _eventPublisher;
    
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        IDomainEventPublisher eventPublisher) : base(options)
    {
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {


        var domainEvents = GetDomainEvents();
        var result = await base.SaveChangesAsync(cancellationToken);
        foreach (var domainEvent in domainEvents)
        {
            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }
        return result;
    }

    private List<IDomainEvent> GetDomainEvents()
    {
        var aggregatesWithEvents = ChangeTracker.Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();
        // Todos los eventos
        var domainEvents = aggregatesWithEvents
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();
        
        aggregatesWithEvents.ForEach(entry => entry.Entity.ClearDomainEvents());
        
        return domainEvents;
    }
    
}