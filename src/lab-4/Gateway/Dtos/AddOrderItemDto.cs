namespace Gateway.Dtos;

public sealed record AddOrderItemDto
{
    public required long ProductId { get; init; }

    public required int Quantity { get; init; }
}