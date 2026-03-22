using Application.Models.Primitives.CompositeObjects;
using Application.Models.Primitives.EntityIds;
using Application.Models.Primitives.Enums;

namespace Application.Models.CreationParameters;

public sealed record OrderHistoryItemCreationParameters
{
    public OrderId OrderId { get; }

    public DateTime CreatedAt { get; }

    public OrderHistoryItemKind Kind { get; }

    public OrderHistoryEvent Payload { get; }

    public OrderHistoryItemCreationParameters(
        OrderId orderId,
        DateTime createdAt,
        OrderHistoryItemKind kind,
        OrderHistoryEvent payload)
    {
        OrderId = orderId;
        CreatedAt = createdAt;
        Kind = kind;
        Payload = payload;
    }
}