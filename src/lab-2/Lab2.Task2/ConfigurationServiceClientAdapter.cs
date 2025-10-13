using Lab2.Task1;

namespace Lab2.Task2;

internal sealed class ConfigurationServiceClientAdapter(IConfigurationServiceClient client) : IConfigurationServiceClientAdapter
{
    public async Task<IDictionary<string, string?>> LoadAsync(CancellationToken cancellationToken = default)
    {
        return await client
            .GetAllConfigurationAsync(cancellationToken)
            .ToDictionaryAsync(
                kv => kv.Key,
                kv => ConvertToNullableString(kv.Value),
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);
    }

    private static string? ConvertToNullableString(string value) => value;
}