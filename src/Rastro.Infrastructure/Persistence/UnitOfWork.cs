using Rastro.Application.Interfaces;
using Rastro.Application.Interfaces.Repositories;
using Rastro.Infrastructure.Persistence.Repositories;

namespace Rastro.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _context;
    public IUserRepository Users { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        // El UnitOfWork es responsable de instanciar los repositorios.
        Users = new UserRepository(_context);
    }
        
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}