using Application.Models.Primitives.EntityIds;

namespace Application.Models.CreationParameters;

public sealed record OrderItemCreationParameters
{
    public OrderId OrderId { get; }

    public ProductId ProductId { get; }

    public int Quantity { get; }

    public bool IsDeleted { get; }

    public OrderItemCreationParameters(
        OrderId orderId,
        ProductId productId,
        int quantity,
        bool isDeleted)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity, nameof(quantity));

        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        IsDeleted = isDeleted;
    }
}
