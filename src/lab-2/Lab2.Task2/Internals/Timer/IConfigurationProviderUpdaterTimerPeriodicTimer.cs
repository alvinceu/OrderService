namespace Lab2.Task2.Internals.Timer;

internal interface IConfigurationProviderUpdaterTimerPeriodicTimer
{
    ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken);
}