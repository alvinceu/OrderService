using Application.Models.Primitives.Enums;

namespace Application.Models.Primitives.CompositeObjects;

public sealed record StateChanged(OrderState Previous, OrderState Current) : OrderHistoryEvent;