using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Lab2.Task1;

public static class ConfigurationServiceClientExtensions
{
    public static IHttpClientBuilder AddConfigurationService(this IServiceCollection collection)
    {
        return collection
            .AddSingleton<IConfigurationServiceClient, ConfigurationServiceClient>()
            .AddHttpClient(ConfigurationServiceClientConstants.HttpClientName);
    }

    public static IHttpClientBuilder AddConfigurationServiceRefit(this IServiceCollection collection)
    {
        return collection
            .AddSingleton<IConfigurationServiceClient, ConfigurationServiceClientRefit>()
            .AddRefitClient<IConfigurationServiceClientRefit>();
    }
}