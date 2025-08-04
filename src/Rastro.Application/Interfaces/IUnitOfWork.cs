using Rastro.Application.Interfaces.Repositories;

namespace Rastro.Application.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}