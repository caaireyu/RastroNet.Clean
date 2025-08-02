using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rastro.Infrastructure.Persistence;


namespace Rastro.Infrastructure.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<IApplicationBuilder> UseDataBaseMigrationAsync(
        this IApplicationBuilder app,
        IHostEnvironment environment)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<MigrationRunner>>();

        try
        {
            if (environment.IsDevelopment())
            {
                // En desarrollo, aplicar migraciones automáticamente
                await context.Database.MigrateAsync();
                logger.LogInformation("Las migraciones se aplicaron correctamente.");
            }
            else
            {
                // En producción, solo verificar que la BD esté actualizada
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogWarning("Hay migraciones pendientes: {Migrations}", 
                        string.Join(", ", pendingMigrations));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ha ocurrido un error al aplicar las migraciones.");
            throw;
        }

        return app;
        
    }
    
}