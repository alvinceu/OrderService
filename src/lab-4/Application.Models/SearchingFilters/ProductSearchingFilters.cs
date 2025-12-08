using Application.Models.Primitives.EntityIds;

namespace Application.Models.SearchingFilters;

public sealed record ProductSearchingFilters
{
    public IReadOnlyCollection<ProductId> Ids { get; init; } = [];

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }

    public string? NameSubstring { get; init; }
}