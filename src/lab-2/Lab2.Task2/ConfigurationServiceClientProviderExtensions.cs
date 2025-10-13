using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lab2.Task2;

public static class ConfigurationServiceClientProviderExtensions
{
    public static IHostApplicationBuilder AddConfigurationProvider(this IHostApplicationBuilder builder, PeriodicTimer timer)
    {
        builder
            .Configuration
            .AddEntityConfiguration();

        builder
            .Services
            .AddSingleton<IConfigurationServiceClientAdapter, ConfigurationServiceClientAdapter>()
            .AddSingleton<IConfigurationUpdaterBackgroundService>(background =>
            {
                IConfiguration config = background.GetRequiredService<IConfiguration>();
                var configRoot = config as IConfigurationRoot;

                ConfigurationServiceClientProvider provider = configRoot?
                    .Providers
                    .OfType<ConfigurationServiceClientProvider>()
                    .FirstOrDefault() ?? throw new InvalidOperationException("ConfigurationServiceClientProvider not found");

                IConfigurationServiceClientAdapter adapter = background.GetRequiredService<IConfigurationServiceClientAdapter>();

                return new ConfigurationUpdaterBackgroundService(provider, adapter, timer);
            });

        return builder;
    }

    private static IConfigurationManager AddEntityConfiguration(this IConfigurationManager manager)
    {
        IConfigurationBuilder configBuilder = manager;
        configBuilder.Add(new ConfigurationServiceClientSource());
        return manager;
    }
}