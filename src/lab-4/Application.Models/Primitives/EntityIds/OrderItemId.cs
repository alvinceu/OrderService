using Application.Models.Commons;

namespace Application.Models.Primitives.EntityIds;

public readonly record struct OrderItemId : IId<OrderItemId>
{
    public long Value { get; }

    public static OrderItemId Create(long value)
    {
        return new OrderItemId(value);
    }

    private OrderItemId(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
        Value = value;
    }
}