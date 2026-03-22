namespace Gateway.Primitives.CompositeObjects;

public sealed record StateChanged : OrderHistoryEvent
{
    public required string PreviousState { get; init; }

    public required string CurrentState { get; init; }
}