namespace Gateway.Dtos;

public sealed record OrderItemAddedDto
{
    public required long OrderItemId { get; init; }

    public required long OrderId { get; init; }

    public required long ProductId { get; init; }

    public required int Quantity { get; init; }

    public required bool IsDeleted { get; init; }
}