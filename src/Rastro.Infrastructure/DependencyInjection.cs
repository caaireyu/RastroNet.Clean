using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rastro.Application.Interfaces;
using Rastro.Infrastructure.Persistence;
using Rastro.Infrastructure.Services;

namespace Rastro.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddPersistence(configuration, environment)
            .AddDomainServices()
            .AddExternalServices(configuration);
        
        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, 
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("PostgreSql")
            ?? throw new InvalidOperationException("La conexión a la base de datos no existe");
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            });
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.ConfigureWarnings(warnings =>
                {
                    // Mostrar warnings útiles en desarrollo
                    warnings.Log(CoreEventId.SensitiveDataLoggingEnabledWarning);
                    warnings.Log(RelationalEventId.MultipleCollectionIncludeWarning);
                    warnings.Log(CoreEventId.FirstWithoutOrderByAndFilterWarning);
                    
                    // Ignorar warnings menos importantes
                    warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
                });
            }
            else if (environment.IsProduction())
            {
                options.EnableServiceProviderCaching();
                options.EnableSensitiveDataLogging(false);
                options.EnableDetailedErrors(false);
                
                options.ConfigureWarnings(warnings =>
                {
                    // En producción, solo errores críticos
                    warnings.Default(WarningBehavior.Ignore);
                    warnings.Throw(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning);
                });
            }
        });
        if (environment.IsProduction())
        {
            services.AddDbContextPool<ApplicationDbContext>( options => options.UseNpgsql(connectionString));   
        }
        
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventPublisher, CortexDomainEventPublisher>();
        return services;
    }

    private static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Servicios externos como:
        // - Email
        // - Storage
        // - Cache
        // - HTTP Clients
        
        // Ejemplo: Redis Cache
        // var redisConnection = configuration.GetConnectionString("Redis");
        // if (!string.IsNullOrEmpty(redisConnection))
        // {
        //     services.AddStackExchangeRedisCache(options =>
        //         options.Configuration = redisConnection);
        // }
        return services;
        
    }
    
}