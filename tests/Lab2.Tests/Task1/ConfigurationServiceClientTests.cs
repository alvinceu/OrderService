using FluentAssertions;
using Lab2.Task1;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lab2.Tests.Task1;

public class ConfigurationServiceClientTests : ConfigurationServiceClientTestsBase
{
    public ConfigurationServiceClientTests() : base(1) { }

    /// <summary>
    /// Integration test that checks the full configuration cycle:
    /// - Getting an empty list of configurations
    /// - Adding a new configuration
    /// - Checking for the presence of the added configuration
    /// - Deleting a configuration
    /// - Checking for data cleanup
    /// </summary>
    /// /// <remarks>
    /// This test is temporarily disabled due to the lack of guarantees of Docker support in the CI environment.
    /// /// </remarks>
    [Fact(Skip = "There is no guarantee that Ci is supported by Docker.")]
    public async Task IntegrationTestHandWritten()
    {
        // Arrange - configuring the DI container and client
        var services = new ServiceCollection();

        services
            .AddConfigurationService()
            .ConfigureHttpClient(clint => clint.BaseAddress = new Uri("http://localhost:8080"));

        ServiceProvider provider = services.BuildServiceProvider();

        IConfigurationServiceClient client = provider.GetRequiredService<IConfigurationServiceClient>();

        // Act & Assert 1: Check the initial state - an empty configuration list
        Paginated<KeyValuePair<string, string>>? result1 = await client.GetConfigurationsAsync(1);

        Assert.NotNull(result1);

        result1
            .Should()
            .BeEquivalentTo(
                new Paginated<KeyValuePair<string, string>>(
                    Enumerable.Empty<KeyValuePair<string, string>>()
                        .ToList()
                        .AsReadOnly()));

        // Act 2: Add a new configuration
        await client.AssignConfigurationAsync("123", "123");

        Paginated<KeyValuePair<string, string>>? result2 = await client.GetConfigurationsAsync(1);

        Assert.NotNull(result2);

        // Assert 2: Check that the configuration was added
        result2
            .Should()
            .BeEquivalentTo(
                new Paginated<KeyValuePair<string, string>>(
                    new List<KeyValuePair<string, string>> { new("123", "123") }.AsReadOnly()),
                options => options.Excluding(x => x.PageToken));

        // Act 3: Delete the configuration
        await client.DeleteConfigurationAsync("123");

        Paginated<KeyValuePair<string, string>>? result3 = await client.GetConfigurationsAsync(1);

        // Assert 3: Check that the configuration has been deleted and the list is empty
        result3
            .Should()
            .BeEquivalentTo(
                new Paginated<KeyValuePair<string, string>>(
                    Enumerable.Empty<KeyValuePair<string, string>>()
                        .ToList()
                        .AsReadOnly()));
    }
}
