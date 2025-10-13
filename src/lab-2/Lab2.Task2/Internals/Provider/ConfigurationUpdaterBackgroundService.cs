using Lab2.Task2.Internals.Timer;
using Microsoft.Extensions.Hosting;

namespace Lab2.Task2.Internals.Provider;

internal sealed class ConfigurationUpdaterBackgroundService(
    ConfigurationServiceClientProvider provider,
    IConfigurationServiceClientAdapter adapter,
    IConfigurationProviderUpdaterTimerPeriodicTimer timer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            IDictionary<string, string?> data = await adapter.LoadAsync(stoppingToken);

            provider.AcceptData(data);
        }
    }
}