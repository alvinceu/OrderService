namespace Gateway.Configurations;

public sealed record GrpcServiceOptions
{
    public string? Address { get; set; }
}