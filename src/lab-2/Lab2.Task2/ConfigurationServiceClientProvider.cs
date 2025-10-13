using Microsoft.Extensions.Configuration;

namespace Lab2.Task2;

public sealed class ConfigurationServiceClientProvider : ConfigurationProvider
{
    internal void AcceptData(IDictionary<string, string?> data)
    {
        if (IsEqualToData(data))
        {
            return;
        }

        Data = data;
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