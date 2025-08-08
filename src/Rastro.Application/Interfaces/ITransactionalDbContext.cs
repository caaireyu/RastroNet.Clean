using Microsoft.EntityFrameworkCore.Storage;

namespace Rastro.Application.Interfaces;

public interface ITransactionalDbContext
{
    IDbContextTransaction? GetCurrentTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    IExecutionStrategy CreateExecutionStrategy();
}