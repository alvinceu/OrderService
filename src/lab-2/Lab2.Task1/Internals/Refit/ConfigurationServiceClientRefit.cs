using Lab2.Task1.Abstractions;
using Lab2.Task1.Commons;
using System.Runtime.CompilerServices;

namespace Lab2.Task1.Internals.Refit;

internal sealed class ConfigurationServiceClientRefit(IConfigurationServiceClientRefit service) : IConfigurationServiceClient
{
    private int MaxPageSize => 200;

    public async IAsyncEnumerable<KeyValuePair<string, string>> GetAllConfigurationAsync([EnumeratorCancellation] CancellationToken ct)
    {
        int pageSize = MaxPageSize;
        string? pageToken = null;

        while (!ct.IsCancellationRequested)
        {
            Paginated<KeyValuePair<string, string>>? response =
                await GetConfigurationsAsync(pageSize, ct, pageToken);

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

    private async Task<Paginated<KeyValuePair<string, string>>?> GetConfigurationsAsync(int pageSize, CancellationToken ct, string? pageToken = null)
    {
        var parameters = new ConfigurationServiceRefitQueryParameters { PageSize = pageSize, PageToken = pageToken };
        return await service.GetConfigurationsAsync(parameters, ct);
    }
}