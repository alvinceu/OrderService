namespace Lab2.Task2;

public interface IConfigurationServiceClientAdapter
{
    Task<IDictionary<string, string?>> LoadAsync(CancellationToken cancellationToken = default);
}