using Application.Models.Commons.Paginations;
using Application.Models.CreationParameters;
using Application.Models.Entities;
using Application.Models.Primitives.EntityIds;
using Application.Models.SearchingFilters;

namespace Application.Abstractions.Repositories;

public interface IOrderHistoryItemRepository
{
    Task<OrderHistoryItem> CreateOrderHistoryItemAsync(
        OrderHistoryItemCreationParameters parameters,
        CancellationToken ct);

    IAsyncEnumerable<OrderHistoryItem> FindOrderHistoryItemsAsync(
        OrderHistoryItemSearchingFilters filters,
        PaginationInfo<OrderHistoryItemId> paginationInfo,
        CancellationToken ct);
}
