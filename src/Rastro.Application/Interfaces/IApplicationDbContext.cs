using Microsoft.EntityFrameworkCore;
using Rastro.Domain.Users;

namespace Rastro.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    
    //Métodos
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
    
    //Transacciones
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    bool HasActiveTransaction { get; }
    
    // ChangeTracker para casos especiales
    Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker ChangeTracker { get; }

}