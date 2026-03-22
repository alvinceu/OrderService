using Lab2.Task1.Commons;
using Refit;

namespace Lab2.Task1.Internals.Refit;

internal interface IConfigurationServiceClientRefit
{
    [Get("/configurations")]
    Task<Paginated<KeyValuePair<string, string>>?> GetConfigurationsAsync([Query] ConfigurationServiceRefitQueryParameters parameters, CancellationToken ct);
}