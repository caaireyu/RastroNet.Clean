
using Cortex.Mediator;
using Cortex.Mediator.Commands;

namespace Rastro.Application.Interfaces;

public interface ITransactionalCommand<TResponse> : ICommand<TResponse>
{
}

public interface ITransactionalCommand : ICommand<Unit>
{
    
}