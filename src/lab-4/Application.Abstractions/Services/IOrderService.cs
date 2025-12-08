using Application.Models.Commons.Paginations;
using Application.Models.Entities;
using Application.Models.Primitives.EntityIds;

namespace Application.Abstractions.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(string createdBy, CancellationToken ct);

    Task<OrderItem> AddOrderItemAsync(OrderId orderId, ProductId productId, int quantity, CancellationToken ct);

    Task RemoveOrderItemAsync(OrderItemId orderItemId, CancellationToken ct);

    Task SetProcessingAsync(OrderId orderId, CancellationToken ct);

    Task SetCompletedAsync(OrderId orderId, CancellationToken ct);

    Task SetCancelledAsync(OrderId orderId, CancellationToken ct);

    Task ReportHistoryEventApprovedAsync(OrderId orderId, CancellationToken ct);

    Task ReportHistoryEventPackingAsync(OrderId orderId, CancellationToken ct);

    Task ReportHistoryEventPackedAsync(OrderId orderId, CancellationToken ct);

    Task ReportHistoryEventInDeliveryAsync(OrderId orderId, CancellationToken ct);

    Task ReportHistoryEventDeliveredAsync(OrderId orderId, CancellationToken ct);

    IAsyncEnumerable<OrderHistoryItem> FindOrderHistoryItemsAsync(
        OrderId orderId,
        PaginationInfo<OrderHistoryItemId> paginationInfo,
        CancellationToken ct);
}
