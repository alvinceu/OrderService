using Lab2.Task2.Internals.Provider;
using Lab2.Task2.Internals.Timer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Lab2.Task2.Extenstions;

public static class ConfigurationServiceClientProviderExtensions
{
    public static IHostApplicationBuilder AddConfigurationProvider(this IHostApplicationBuilder builder)
    {
        builder
            .Configuration
            .AddEntityConfiguration();

        builder
            .Services
            .Configure<TimerOptions>(builder.Configuration.GetSection(TimerOptions.SectionName));

        builder
            .Services
            .AddSingleton<IConfigurationServiceClientAdapter, ConfigurationServiceClientAdapter>()
            .AddHostedService(provider =>
            {
                IConfiguration config = provider.GetRequiredService<IConfiguration>();

                IOptions<TimerOptions> options = provider.GetRequiredService<IOptions<TimerOptions>>();

                var configRoot = config as IConfigurationRoot;

                ConfigurationServiceClientProvider configurationServiceProvider = configRoot?
                    .Providers
                    .OfType<ConfigurationServiceClientProvider>()
                    .FirstOrDefault() ?? throw new InvalidOperationException("ConfigurationServiceClientProvider not found");

                IConfigurationServiceClientAdapter adapter = provider.GetRequiredService<IConfigurationServiceClientAdapter>();

                return new ConfigurationUpdaterBackgroundService(configurationServiceProvider, adapter, options.Value);
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