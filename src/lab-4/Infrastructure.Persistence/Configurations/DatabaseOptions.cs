namespace Infrastructure.Persistence.Configurations;

public sealed record DatabaseOptions
{
    public string? ConnectionString { get; set; }
}