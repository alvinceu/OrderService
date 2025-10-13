using Microsoft.Extensions.Options;

namespace Lab2.Task2.Internals.Timer;

internal sealed class ConfigurationProviderUpdaterTimerPeriodicTimer : IConfigurationProviderUpdaterTimerPeriodicTimer, IDisposable
{
    private readonly PeriodicTimer _timer;

    public ConfigurationProviderUpdaterTimerPeriodicTimer(IOptions<TimerOptions> options)
    {
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(options.Value.IntervalSeconds));
    }

    public async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken)
    {
        return await _timer.WaitForNextTickAsync(cancellationToken);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}