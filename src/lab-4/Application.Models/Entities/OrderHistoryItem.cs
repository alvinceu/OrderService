using Application.Models.Commons;
using Application.Models.Primitives.CompositeObjects;
using Application.Models.Primitives.EntityIds;
using Application.Models.Primitives.Enums;

namespace Application.Models.Entities;

public sealed class OrderHistoryItem : Entity<OrderHistoryItemId>
{
    public OrderId OrderId { get; }

    public DateTime CreatedAt { get; }

    public OrderHistoryItemKind Kind { get; }

    public OrderHistoryEvent Payload { get; }

    public OrderHistoryItem(
        OrderHistoryItemId id,
        OrderId orderId,
        DateTime createdAt,
        OrderHistoryItemKind kind,
        OrderHistoryEvent payload)
        : base(id)
    {
        OrderId = orderId;
        CreatedAt = createdAt;
        Kind = kind;
        Payload = payload;
    }
}