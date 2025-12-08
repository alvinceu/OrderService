namespace Lab2.Task1.Commons;

internal sealed record ConfigurationServiceOptions
{
    public const string SectionName = "ConfigurationService";

    public required string Url { get; set; }
}