using Application.Models.Commons;

namespace Application.Models.Primitives.EntityIds;

public readonly record struct OrderHistoryItemId : IId<OrderHistoryItemId>
{
    public long Value { get; }

    public static OrderHistoryItemId Create(long value)
    {
        return new OrderHistoryItemId(value);
    }

    private OrderHistoryItemId(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
        Value = value;
    }
}