namespace Lab2.Task2;

internal interface IConfigurationProviderUpdaterTimerPeriodicTimer
{
    ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken);
}