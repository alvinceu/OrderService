using Application.Models.Commons;

namespace Application.Models.Primitives.EntityIds;

public readonly record struct OrderId : IId<OrderId>
{
    public long Value { get; }

    public static OrderId Create(long value)
    {
        return new OrderId(value);
    }

    private OrderId(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
        Value = value;
    }
}