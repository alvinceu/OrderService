using Microsoft.Extensions.Configuration;

namespace Lab2.Task2;

public sealed class ConfigurationServiceClientSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new ConfigurationServiceClientProvider();
    }
}