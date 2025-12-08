namespace Gateway.Primitives.CompositeObjects;

public sealed record ItemRemoved : OrderHistoryEvent
{
    public required long OrderItemId { get; init; }
}