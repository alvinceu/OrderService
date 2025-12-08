using Lab2.Task1.Abstractions;
using Lab2.Task1.Commons;
using Lab2.Task1.Internals.HandMade;
using Lab2.Task1.Internals.Refit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Refit;

namespace Lab2.Task1.Extensions;

public static class ConfigurationServiceClientExtensions
{
    public static string HttpClientName => "ConfigurationService1";

    public static IHttpClientBuilder AddConfigurationService(this IHostApplicationBuilder builder)
    {
        builder
            .Services
            .Configure<ConfigurationServiceOptions>(
                builder
                    .Configuration
                    .GetSection(ConfigurationServiceOptions.SectionName));

        builder
            .Services
            .AddSingleton<IConfigurationServiceClient, ConfigurationServiceClient>();

        return builder
            .Services
            .AddHttpClient(HttpClientName, (sp, client) =>
            {
                ConfigurationServiceOptions options = sp.GetRequiredService<IOptions<ConfigurationServiceOptions>>().Value;

                client.BaseAddress = new Uri(options.Url);
            });
    }

    public static IHttpClientBuilder AddConfigurationServiceRefit(this IHostApplicationBuilder builder)
    {
        builder
            .Services
            .Configure<ConfigurationServiceOptions>(
                builder
                    .Configuration
                    .GetSection(ConfigurationServiceOptions.SectionName));

        builder
            .Services
            .AddSingleton<IConfigurationServiceClient, ConfigurationServiceClientRefit>();

        return builder
            .Services
            .AddRefitClient<IConfigurationServiceClientRefit>()
            .ConfigureHttpClient((sp, client) =>
            {
                ConfigurationServiceOptions options = sp.GetRequiredService<IOptions<ConfigurationServiceOptions>>().Value;

                client.BaseAddress = new Uri(options.Url);
            });
    }
}