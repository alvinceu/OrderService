namespace Gateway.Dtos;

public sealed record StartOrderPackingDto
{
    public required string PackingBy { get; init; }
}