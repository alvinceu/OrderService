namespace Lab2.Task1.Abstractions;

public interface IConfigurationServiceClient
{
    IAsyncEnumerable<KeyValuePair<string, string>> GetAllConfigurationAsync(CancellationToken cancellationToken = default);
}