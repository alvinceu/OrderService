namespace Lab2.Task1;

public interface IConfigurationServiceClient
{
    Task<Paginated<KeyValuePair<string, string>>?> GetConfigurationsAsync(int pageSize, string? pageToken = null, CancellationToken cancellationToken = default);

    Task AssignConfigurationAsync(string key, string value, CancellationToken cancellationToken = default);

    Task DeleteConfigurationAsync(string key, CancellationToken cancellationToken = default);

    IAsyncEnumerable<KeyValuePair<string, string>> GetAllConfigurationAsync(CancellationToken cancellationToken = default);
}