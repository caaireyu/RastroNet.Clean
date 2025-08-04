using Cortex.Mediator.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        return services;
    }
    
}