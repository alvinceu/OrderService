using Microsoft.Extensions.Configuration;

namespace Lab2.Task2.Internals.Provider;

public sealed class ConfigurationServiceClientProvider : ConfigurationProvider
{
    internal void AcceptData(IDictionary<string, string?> data)
    {
        if (IsEqualToData(data))
        {
            return;
        }

        foreach (KeyValuePair<string, string?> kvp in data)
        {
            if (!Data.TryGetValue(kvp.Key, out string? currentValue) ||
                !string.Equals(currentValue, kvp.Value, StringComparison.OrdinalIgnoreCase))
            {
                Data[kvp.Key] = kvp.Value;
            }
        }

        var keysToRemove = Data.Keys.Except(data.Keys).ToList();
        foreach (string? key in keysToRemove)
        {
            Data.Remove(key);
        }

        OnReload();

        return;

        bool IsEqualToData(IDictionary<string, string?> other)
        {
            if (other.Count != Data.Count)
            {
                return false;
            }

            if (other.Keys.Except(Data.Keys, StringComparer.OrdinalIgnoreCase).Any())
            {
                return false;
            }

            if (Data.Keys.Except(other.Keys, StringComparer.OrdinalIgnoreCase).Any())
            {
                return false;
            }

            return other.All(kv => kv.Value == Data[kv.Key]);
        }
    }
}