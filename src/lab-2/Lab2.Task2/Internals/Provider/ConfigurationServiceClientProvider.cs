using Microsoft.Extensions.Configuration;

namespace Lab2.Task2.Internals.Provider;

internal sealed class ConfigurationServiceClientProvider : ConfigurationProvider
{
    internal void AcceptData(IDictionary<string, string?> data)
    {
        bool hasChanges = false;

        foreach (KeyValuePair<string, string?> kvp in data)
        {
            if (!Data.TryGetValue(kvp.Key, out string? currentValue) ||
                !string.Equals(currentValue, kvp.Value, StringComparison.OrdinalIgnoreCase))
            {
                Data[kvp.Key] = kvp.Value;
                hasChanges = true;
            }
        }

        var keysToRemove = Data.Keys.Except(data.Keys).ToList();
        if (keysToRemove.Count > 0)
        {
            hasChanges = true;
        }

        foreach (string key in keysToRemove)
        {
            Data.Remove(key);
        }

        if (hasChanges)
        {
            OnReload();
        }
    }
}