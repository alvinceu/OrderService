namespace Lab2.Task2.Internals.Provider;

internal interface IConfigurationServiceClientAdapter
{
    Task<IDictionary<string, string?>> LoadAsync(CancellationToken ct);
}