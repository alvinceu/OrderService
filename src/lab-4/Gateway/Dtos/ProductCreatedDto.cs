namespace Gateway.Dtos;

public sealed record ProductCreatedDto
{
    public required long ProductId { get; init; }

    public required string Name { get; init; }

    public required string Price { get; init; }
}