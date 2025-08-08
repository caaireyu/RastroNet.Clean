using Microsoft.EntityFrameworkCore;
using Cortex.Mediator.Commands;
using Microsoft.Extensions.Logging;
using Rastro.Application.Interfaces;
namespace Rastro.Application.Behaviors;

public class TransactionalBehavior<TRequest, TResponse> : ICommandPipelineBehavior<TRequest, TResponse>
    where TRequest: ICommand<TResponse>
{
    private readonly ITransactionalDbContext _dbContext;
    private readonly ILogger<TransactionalBehavior<TRequest, TResponse>> _logger;

    public TransactionalBehavior(ITransactionalDbContext dbContext,
        ILogger<TransactionalBehavior<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<TResponse> Handle(TRequest command, CommandHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var isTransactional = command.GetType().GetInterfaces()
            .Any(i => i == typeof(ITransactionalCommand) || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITransactionalCommand<>)));
        if (!isTransactional)
        {
            return await next();
        }

        if (_dbContext.GetCurrentTransaction() is not null)
        {
            return await next();
        }

        var strategy = _dbContext.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async (ct) =>
        {
            await using var transaction = await _dbContext.BeginTransactionAsync(ct);
            _logger.LogInformation("------ Iniciando transacción para {CommandName} ({@Command})",
                typeof(TRequest).Name, command);
            try
            {
                var response = await next();
                await transaction.CommitAsync(ct);
                _logger.LogInformation("------ Transacción para {CommandName} confirmada (commited)",
                    typeof(TRequest).Name);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"------ Ha ocurrido un error Transacción para {CommandName}. Revirtiendo ", typeof(TRequest).Name);
                await transaction.RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);

    }
}