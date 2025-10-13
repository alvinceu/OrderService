using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;
using Xunit;

namespace Lab2.Tests.Task1;

public class ConfigurationServiceClientTestsBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly INetwork _network;
    private readonly IContainer _container;

    protected ConfigurationServiceClientTestsBase(int i)
    {
        _network = new NetworkBuilder()
            .WithDriver(NetworkDriver.Bridge)
            .Build();

        _postgresContainer = new PostgreSqlBuilder()
            .WithName($"postgres{i}")
            .WithImage("postgres:latest")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithDatabase("postgres")
            .WithPortBinding(5432 + i, 5432)
            .WithNetwork(_network)
            .Build();

        _container = new ContainerBuilder()
            .WithImage("ghcr.io/is-csms-y27/lab-2-tools:master")
            .WithEnvironment("Persistence__Postgres__Host", $"postgres{i}")
            .WithPortBinding(8080 + i, 8080)
            .WithNetwork(_network)
            .DependsOn(_postgresContainer)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilCommandIsCompleted("sleep", "1s"))
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _network.CreateAsync();
        await _postgresContainer.StartAsync();
        await _container.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await _network.DisposeAsync();
        await _container.DisposeAsync();
    }
}