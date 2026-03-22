namespace Gateway.Dtos;

public sealed record CreateProductDto
{
    public required string Name { get; init; }

    public required string Price { get; init; }
}