using Lab2.Task2.Internals.Provider;
using Microsoft.Extensions.Configuration;

namespace Lab2.Task2.Extenstions;

public sealed class ConfigurationServiceClientSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new ConfigurationServiceClientProvider();
    }
}