namespace Gateway.Dtos;

public sealed record CreateOrderDto
{
    public required string CreatedBy { get; init; }
}