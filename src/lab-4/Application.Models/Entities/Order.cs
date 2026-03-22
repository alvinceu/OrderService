using Application.Models.Commons;
using Application.Models.Primitives.EntityIds;
using Application.Models.Primitives.Enums;

namespace Application.Models.Entities;

public sealed class Order : Entity<OrderId>
{
    public OrderState State { get; }

    public DateTime CreatedAt { get; }

    public string CreatedBy { get; }

    public Order(
        OrderId id,
        OrderState state,
        DateTime createdAt,
        string createdBy)
        : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        State = state;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
    }
}