using Application.Models.Primitives.EntityIds;
using Application.Models.Primitives.Enums;

namespace Application.Models.SearchingFilters;

public sealed record OrderSearchingFilters
{
    public IReadOnlyCollection<OrderId> OrderIds { get; init; } = [];

    public OrderState? State { get; init; }

    public string? Author { get; init; }
}