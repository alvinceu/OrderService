using FluentMigrator.Runner;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.Extensions;

public static class MigrationExtensions
{
    public static void AddMigration(this IServiceCollection services)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(runnerBuilder =>
            {
                runnerBuilder
                    .AddPostgres()
                    .WithGlobalConnectionString(serviceProvider =>
                    {
                        DatabaseOptions dbOptions =
                            serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

                        ArgumentException.ThrowIfNullOrWhiteSpace(dbOptions.ConnectionString);

                        return dbOptions.ConnectionString;
                    })
                    .ScanIn(typeof(InitialMigration).Assembly)
                    .For
                    .Migrations();
            });
    }

    public static async Task UpdateDatabase(this IServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        IMigrationRunner migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        migrationRunner.MigrateUp();
    }
}