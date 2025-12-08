using Lab2.Task1.Abstractions;

namespace Lab2.Task2.Internals.Provider;

internal sealed class ConfigurationServiceClientAdapter(IConfigurationServiceClient client) : IConfigurationServiceClientAdapter
{
    public async Task<IDictionary<string, string?>> LoadAsync(CancellationToken ct)
    {
        return await client
            .GetAllConfigurationAsync(ct)
            .ToDictionaryAsync(
                kv => kv.Key,
                kv => ConvertToNullableString(kv.Value),
                StringComparer.OrdinalIgnoreCase,
                ct);
    }

    private static string? ConvertToNullableString(string value) => value;
}