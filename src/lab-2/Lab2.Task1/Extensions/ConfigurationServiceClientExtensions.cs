using Lab2.Task1.Abstractions;
using Lab2.Task1.Internals.HandMade;
using Lab2.Task1.Internals.Refit;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Lab2.Task1.Extensions;

public static class ConfigurationServiceClientExtensions
{
    public static string HttpClientName => "ConfigurationService1";

    public static IHttpClientBuilder AddConfigurationService(this IServiceCollection collection)
    {
        return collection
            .AddSingleton<IConfigurationServiceClient, ConfigurationServiceClient>()
            .AddHttpClient(HttpClientName)
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://localhost:8080"));
    }

    public static IHttpClientBuilder AddConfigurationServiceRefit(this IServiceCollection collection)
    {
        return collection
            .AddSingleton<IConfigurationServiceClient, ConfigurationServiceClientRefit>()
            .AddRefitClient<IConfigurationServiceClientRefit>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://localhost:8080"));
    }
}