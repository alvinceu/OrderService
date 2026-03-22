using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Extensions;
using Microsoft.Extensions.Options;

namespace Presentation.Grpc.Program;

internal sealed class DatabaseStartUpBackgroundService(
    IServiceProvider provider,
    IOptionsMonitor<DatabaseStartUpOptions> delayOptions) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await WaitAndInitializeAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    private async Task WaitAndInitializeAsync(CancellationToken token)
    {
        IOptionsMonitor<DatabaseOptions> monitor = provider
            .GetRequiredService<IOptionsMonitor<DatabaseOptions>>();

        while (!token.IsCancellationRequested)
        {
            string? connection = monitor.CurrentValue.ConnectionString;

            if (!string.IsNullOrWhiteSpace(connection))
            {
                await provider.UpdateDatabase();
                break;
            }

            await Task.Delay(TimeSpan.FromSeconds(delayOptions.CurrentValue.IntervalSeconds), token);
        }
    }
}