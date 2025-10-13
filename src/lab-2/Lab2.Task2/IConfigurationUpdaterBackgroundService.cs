namespace Lab2.Task2;

public interface IConfigurationUpdaterBackgroundService
{
    void Start(CancellationToken cancellationToken = default);
}