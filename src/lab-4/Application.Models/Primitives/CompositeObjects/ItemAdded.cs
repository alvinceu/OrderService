using Application.Models.Primitives.EntityIds;

namespace Application.Models.Primitives.CompositeObjects;

public sealed record ItemAdded(OrderItemId ItemId) : OrderHistoryEvent;