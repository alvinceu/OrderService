using Application.Models.Commons;

namespace Application.Models.Primitives.EntityIds;

public readonly record struct ProductId : IId<ProductId>
{
    public long Value { get; }

    public static ProductId Create(long value)
    {
        return new ProductId(value);
    }

    private ProductId(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
        Value = value;
    }
}