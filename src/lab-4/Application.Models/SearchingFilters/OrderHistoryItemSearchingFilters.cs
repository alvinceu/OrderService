using Application.Models.Primitives.EntityIds;
using Application.Models.Primitives.Enums;

namespace Application.Models.SearchingFilters;

public sealed record OrderHistoryItemSearchingFilters
{
    public IReadOnlyCollection<OrderId> OrderIds { get; set; } = [];

    public OrderHistoryItemKind? Kind { get; init; }
}