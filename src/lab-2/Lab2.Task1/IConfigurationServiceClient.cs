namespace Lab2.Task1;

public interface IConfigurationServiceClient
{
    IAsyncEnumerable<KeyValuePair<string, string>> GetAllConfigurationAsync(CancellationToken cancellationToken = default);
}