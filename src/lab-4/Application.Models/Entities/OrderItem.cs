using Application.Models.Commons;
using Application.Models.Primitives.EntityIds;

namespace Application.Models.Entities;

public sealed class OrderItem : Entity<OrderItemId>
{
    public OrderId OrderId { get; }

    public ProductId ProductId { get; }

    public int Quantity { get; }

    public bool IsDeleted { get; }

    public OrderItem(
        OrderItemId id,
        OrderId orderId,
        ProductId productId,
        int quantity,
        bool deleted)
        : base(id)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity, nameof(quantity));

        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        IsDeleted = deleted;
    }
}