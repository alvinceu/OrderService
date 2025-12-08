using Gateway.Primitives.Enums;

namespace Gateway.Primitives.CompositeObjects;

public sealed record ProcessingStageChanged : OrderHistoryEvent
{
    public required OrderProcessingStage Stage { get; init; }
}