using Lab2.Task2.Internals.Timer;
using Microsoft.Extensions.Hosting;

namespace Lab2.Task2.Internals.Provider;

internal sealed class ConfigurationUpdaterBackgroundService(
    ConfigurationServiceClientProvider provider,
    IConfigurationServiceClientAdapter adapter,
    TimerOptions options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(options.IntervalSeconds));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            IDictionary<string, string?> data = await adapter.LoadAsync(stoppingToken);

            provider.AcceptData(data);
        }
    }
}