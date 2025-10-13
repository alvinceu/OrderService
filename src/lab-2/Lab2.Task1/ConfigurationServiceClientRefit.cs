using System.Runtime.CompilerServices;

namespace Lab2.Task1;

internal sealed class ConfigurationServiceClientRefit(IConfigurationServiceClientRefit service) : IConfigurationServiceClient
{
    public async Task<Paginated<KeyValuePair<string, string>>?> GetConfigurationsAsync(int pageSize, string? pageToken = null, CancellationToken cancellationToken = default)
    {
        var parameters = new ConfigurationServiceRefitQueryParameters { PageSize = pageSize, PageToken = pageToken };
        return await service.GetConfigurationsAsync(parameters, cancellationToken);
    }

    public async Task AssignConfigurationAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        var body = new KeyValuePair<string, string>(key, value);
        await service.AssignConfigurationAsync(body, cancellationToken);
    }

    public async Task DeleteConfigurationAsync(string key, CancellationToken cancellationToken = default)
    {
        await service.DeleteConfigurationAsync(key, cancellationToken);
    }

    public async IAsyncEnumerable<KeyValuePair<string, string>> GetAllConfigurationAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int pageSize = ConfigurationServiceClientConstants.MaxPageSize;
        string? pageToken = null;

        while (!cancellationToken.IsCancellationRequested)
        {
            Paginated<KeyValuePair<string, string>>? response =
                await GetConfigurationsAsync(pageSize, pageToken, cancellationToken);

            if (response is null)
            {
                yield break;
            }

            foreach (KeyValuePair<string, string> kv in response.Items)
            {
                yield return kv;
            }

            if (response.PageToken is null)
            {
                yield break;
            }

            pageToken = response.PageToken;
        }
    }
}