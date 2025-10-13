using Refit;

namespace Lab2.Task1;

internal interface IConfigurationServiceClientRefit
{
    [Get("/configurations")]
    Task<Paginated<KeyValuePair<string, string>>?> GetConfigurationsAsync([Query] ConfigurationServiceRefitQueryParameters parameters, CancellationToken cancellationToken = default);
}