using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace Lab2.Task1;

internal sealed class ConfigurationServiceClient(IHttpClientFactory factory) : IConfigurationServiceClient
{
    private int MaxPageSize => 200;

    public async IAsyncEnumerable<KeyValuePair<string, string>> GetAllConfigurationAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        int pageSize = MaxPageSize;
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

    private async Task<Paginated<KeyValuePair<string, string>>?> GetConfigurationsAsync(int pageSize, string? pageToken = null, CancellationToken cancellationToken = default)
    {
        HttpClient client = factory.CreateClient(ConfigurationServiceClientExtensions.HttpClientName);
        string query = pageToken is null ? $"?pageSize={pageSize}" : $"?pageSize={pageSize}&pageToken={pageToken}";
        return await client.GetFromJsonAsync<Paginated<KeyValuePair<string, string>>>($"/configurations{query}", cancellationToken: cancellationToken);
    }
}
