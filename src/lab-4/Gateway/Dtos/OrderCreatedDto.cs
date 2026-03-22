namespace Gateway.Dtos;

public sealed record OrderCreatedDto
{
    public required long OrderId { get; init; }

    public required string State { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required string CreatedBy { get; init; }
}