using Gateway.Primitives.CompositeObjects;

namespace Gateway.Dtos;

public sealed record OrderHistoryItemDto
{
    public required long OrderHistoryItemId { get; init; }

    public required long OrderId { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required string Kind { get; init; }

    public required OrderHistoryEvent Payload { get; init; }
}