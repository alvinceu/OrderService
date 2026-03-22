using Application.Models.Primitives.Enums;

namespace Application.Models.Primitives.CompositeObjects;

public sealed record ProcessingStageChanged(ProcessingStage Stage) : OrderHistoryEvent;