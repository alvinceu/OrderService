using FluentAssertions;
using Lab2.Task1.Extensions;
using Lab2.Task2.Extenstions;
using Lab2.Task2.Internals.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Lab2.Tests.Task2;

public class ConfigurationServiceClientProviderTests
{
    [Fact]
    public void ShouldAddConfigurationWhenProviderIsEmptyAndNewConfigAdded()
    {
        // Arrange 1 : Configure services
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder
            .Services
            .AddConfigurationServiceRefit();

        builder
            .AddConfigurationProvider();

        using IHost host = builder.Build();

        // Arrange 2 : Pull the necessary services
        IConfiguration config = host
            .Services
            .GetRequiredService<IConfiguration>();

        var configRoot = config as IConfigurationRoot;

        ConfigurationServiceClientProvider provider = configRoot?
            .Providers
            .OfType<ConfigurationServiceClientProvider>()
            .FirstOrDefault() ?? throw new InvalidOperationException("ConfigurationServiceClientProvider not found");

        // Arrange 3 : to track changes
        IChangeToken token = config.GetReloadToken();

        // Act
        provider.AcceptData(new Dictionary<string, string?>()
        {
            { "str1", "str2" },
        });

        // Assert
        config["str1"].Should().Be("str2");
        token.HasChanged.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotUpdateConfigurationWhenSameConfigAddedToProvider()
    {
        // Arrange 1 : Configure services
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder
            .Services
            .AddConfigurationServiceRefit();

        builder
            .AddConfigurationProvider();

        using IHost host = builder.Build();

        // Arrange 2 : Pull the necessary services
        IConfiguration config = host
            .Services
            .GetRequiredService<IConfiguration>();

        var configRoot = config as IConfigurationRoot;

        ConfigurationServiceClientProvider provider = configRoot?
            .Providers
            .OfType<ConfigurationServiceClientProvider>()
            .FirstOrDefault() ?? throw new InvalidOperationException("ConfigurationServiceClientProvider not found");

        // Arrange 3 : Pre-load data config
        provider.AcceptData(new Dictionary<string, string?>()
        {
            { "str1", "str2" },
        });

        // Arrange 4 : to track changes
        IChangeToken token = config.GetReloadToken();

        // Act
        provider.AcceptData(new Dictionary<string, string?>()
        {
            { "str1", "str2" },
        });

        // Assert
        token.HasChanged.Should().BeFalse();
    }

    [Fact]
    public void ShouldUpdateValueWhenConfigWithSameKeyButDifferentValueAdded()
    {
        // Arrange 1 : Configure services
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder
            .AddConfigurationProvider();

        builder
            .Services
            .AddConfigurationServiceRefit();

        using IHost host = builder.Build();

        // Arrange 2 : Pull the necessary services
        IConfiguration config = host
            .Services
            .GetRequiredService<IConfiguration>();

        var configRoot = config as IConfigurationRoot;

        ConfigurationServiceClientProvider provider = configRoot?
            .Providers
            .OfType<ConfigurationServiceClientProvider>()
            .FirstOrDefault() ?? throw new InvalidOperationException("ConfigurationServiceClientProvider not found");

        // Arrange 3 : Pre-load data config
        provider.AcceptData(new Dictionary<string, string?>()
        {
            { "str1", "str2" },
        });

        // Arrange 4 : to track changes
        IChangeToken token = config.GetReloadToken();

        // Act
        provider.AcceptData(new Dictionary<string, string?>()
        {
            { "str1", "new_str" },
        });

        // Assert
        token.HasChanged.Should().BeTrue();

        config["str1"].Should().Be("new_str");
    }

    [Fact]
    public void ShouldClearAllConfigurationsWhenEmptyCollectionProvided()
    {
        // Arrange 1 : Configure services
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder
            .Services
            .AddConfigurationServiceRefit()
            .ConfigureHttpClient(clint => clint.BaseAddress = new Uri("http://localhost:8080"));

        builder
            .AddConfigurationProvider();

        using IHost host = builder.Build();

        // Arrange 2 : Pull the necessary services
        IConfiguration config = host
            .Services
            .GetRequiredService<IConfiguration>();

        var configRoot = config as IConfigurationRoot;

        ConfigurationServiceClientProvider provider = configRoot?
            .Providers
            .OfType<ConfigurationServiceClientProvider>()
            .FirstOrDefault() ?? throw new InvalidOperationException("ConfigurationServiceClientProvider not found");

        // Arrange 3 : Pre-load data config
        provider.AcceptData(new Dictionary<string, string?>()
        {
            { "str1", "str2" },
        });

        // Arrange 4: to track changes
        IChangeToken token = config.GetReloadToken();

        // Act
        provider.AcceptData(new Dictionary<string, string?>());

        // Assert
        token.HasChanged.Should().BeTrue();

        config["str1"].Should().Be(null);
    }
}