using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            .AddDomainServices(configuration)
            .AddExternalServices(configuration);
        
        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, 
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("PostgreSql")
            ?? throw new InvalidOperationException("La conexión a la base de datos no existe");

        services.AddDbContext<ApplicationDbContext>((servicesProvider, options) =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: []);
                npgsqlOptions.CommandTimeout(30);
            });
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.ConfigureWarnings(warnings =>
                {
                    warnings.Log(
                        CoreEventId.SensitiveDataLoggingEnabledWarning,
                        RelationalEventId.MultipleCollectionIncludeWarning,
                        CoreEventId.FirstWithoutOrderByAndFilterWarning
                        );
                    warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
                });
            }
            else
            {

                options.EnableSensitiveDataLogging(false);
                options.EnableDetailedErrors(false);
                options.ConfigureWarnings(warnings =>
                {
                    warnings.Default(WarningBehavior.Ignore);
                    warnings.Throw(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning);
                });
            }
            

        });
        services.AddScoped<ITransactionalDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
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