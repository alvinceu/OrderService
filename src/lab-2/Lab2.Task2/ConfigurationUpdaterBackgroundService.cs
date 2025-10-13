namespace Lab2.Task2;

internal sealed class ConfigurationUpdaterBackgroundService(
    ConfigurationServiceClientProvider provider,
    IConfigurationServiceClientAdapter adapter,
    PeriodicTimer timer) : IConfigurationUpdaterBackgroundService
{
    private bool _isStarted = false;

    public void Start(CancellationToken cancellationToken = default)
    {
        if (Interlocked.Exchange(ref _isStarted, true))
        {
            return;
        }

        Task.Run(UpdatePeriodically, cancellationToken);

        return;

        async Task? UpdatePeriodically()
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                IDictionary<string, string?> data = await adapter.LoadAsync(cancellationToken);

                provider.AcceptData(data);
            }
        }
    }
}