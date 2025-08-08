using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rastro.Application.Behaviors;
using Rastro.Application.Interfaces;

namespace Rastro.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCortexMediator(
            configuration, [typeof(DependencyInjection)],
            configure: options =>
            {
                options.AddDefaultBehaviors();
            });
        services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(TransactionalBehavior<,>));
        return services;
    }
    
}