using Application.Models.Commons.Paginations;
using Application.Models.CreationParameters;
using Application.Models.Entities;
using Application.Models.Primitives.EntityIds;
using Application.Models.SearchingFilters;

namespace Application.Abstractions.Repositories;

public interface IOrderItemRepository
{
    Task<OrderItem> CreateOrderItemAsync(OrderItemCreationParameters parameters, CancellationToken ct);

    Task SoftDeleteOrderItemAsync(OrderItem orderItem, CancellationToken ct);

    Task<OrderItem?> FindOrderItemByIdAsync(OrderItemId orderItemId, CancellationToken ct);

    IAsyncEnumerable<OrderItem> FindOrderItemsAsync(
        OrderItemSearchingFilters filters,
        PaginationInfo<OrderItemId> paginationInfo,
        CancellationToken ct);
}