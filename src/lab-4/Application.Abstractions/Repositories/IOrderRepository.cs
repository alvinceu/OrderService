using Application.Models.Commons.Paginations;
using Application.Models.CreationParameters;
using Application.Models.Entities;
using Application.Models.Primitives.EntityIds;
using Application.Models.SearchingFilters;

namespace Application.Abstractions.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateOrderAsync(OrderCreationParameters parameters, CancellationToken ct);

    Task ChangeStatusAsync(Order order, CancellationToken ct);

    Task<Order?> FindOrderByIdAsync(OrderId orderId, CancellationToken ct);

    IAsyncEnumerable<Order> FindOrdersAsync(
        OrderSearchingFilters filters,
        PaginationInfo<OrderId> paginationInfo,
        CancellationToken ct);
}