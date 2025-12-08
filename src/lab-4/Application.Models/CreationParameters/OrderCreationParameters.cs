using Application.Models.Primitives.Enums;

namespace Application.Models.CreationParameters;

public sealed record OrderCreationParameters
{
    public OrderState State { get; }

    public DateTime CreatedAt { get; }

    public string CreatedBy { get; }

    public OrderCreationParameters(
        OrderState state,
        DateTime createdAt,
        string createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        State = state;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
    }
}
