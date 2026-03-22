using Application.Models.Primitives.EntityIds;

namespace Application.Models.SearchingFilters;

public sealed record OrderItemSearchingFilters
{
    public IReadOnlyCollection<OrderId> OrderIds { get; set; } = [];

    public IReadOnlyCollection<ProductId> ProductIds { get; set; } = [];

    public bool? IsDeleted { get; init; }
}