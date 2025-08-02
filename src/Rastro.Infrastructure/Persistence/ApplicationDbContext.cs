using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Rastro.Application.Interfaces;
using Rastro.Domain.Primitives;
using Rastro.Domain.Users;

namespace Rastro.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IDomainEventPublisher _eventPublisher;
    private IDbContextTransaction? _currentTransaction;
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        IDomainEventPublisher eventPublisher) : base(options)
    {
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    }
    
    public bool HasActiveTransaction => _currentTransaction != null;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("La transacción ya está en progreso.");
        }
        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
        
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No existe ninguna transacción en prograso.");
        }
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await RollbackTransactionAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No existe ninguna transacción activa para hacer rollback");
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }
    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Obtener agregados con eventos ANTES de guardar
        var aggregatesWithEvents = ChangeTracker.Entries<AggregateRoot>()
            .Where(x => x.Entity.GetDomainEvents().Any())
            .ToList();
        // Todos los eventos
        var domainEvents = aggregatesWithEvents
                .SelectMany(x => x.Entity.GetDomainEvents())
                .ToList();
        // Limpiar eventos de los agregados ANTES de guardar
        aggregatesWithEvents.ForEach(entry => entry.Entity.ClearDomainEvents());
        // Guardar cambios en la base de datos
        var result = await base.SaveChangesAsync(cancellationToken);

        // Publicar eventos DESPUÉS de guardar exitosamente
        await PublishDomainEventsAsync(domainEvents, cancellationToken);

        return result;
    }
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        // Obtener agregados con eventos ANTES de guardar
        var aggregatesWithEvents = ChangeTracker.Entries<AggregateRoot>()
            .Where(x => x.Entity.GetDomainEvents().Any())
            .ToList();

        // Obtener todos los eventos
        var domainEvents = aggregatesWithEvents
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();

        // Limpiar eventos de los agregados ANTES de guardar
        aggregatesWithEvents.ForEach(entry => entry.Entity.ClearDomainEvents());

        // Guardar cambios en la base de datos
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        // Publicar eventos DESPUÉS de guardar exitosamente
        await PublishDomainEventsAsync(domainEvents, cancellationToken);

        return result;
    }
    private async Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }
    }
    public override void Dispose()
    {
        _currentTransaction?.Dispose();
        base.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
        }
        await base.DisposeAsync();
    }
}