using Application.Models.Primitives.EntityIds;

namespace Application.Models.Primitives.CompositeObjects;

public sealed record ItemRemoved(OrderItemId ItemId) : OrderHistoryEvent;