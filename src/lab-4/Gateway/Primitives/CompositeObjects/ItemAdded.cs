namespace Gateway.Primitives.CompositeObjects;

public sealed record ItemAdded : OrderHistoryEvent
{
    public required long OrderItemId { get; init; }
}