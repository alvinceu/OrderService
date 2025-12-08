using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Models.Commons.Paginations;
using Application.Models.CreationParameters;
using Application.Models.Entities;
using Application.Models.Primitives.CompositeObjects;
using Application.Models.Primitives.EntityIds;
using Application.Models.Primitives.Enums;
using Application.Models.SearchingFilters;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace Application.Services;

internal sealed class OrderService(
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository,
    IOrderHistoryItemRepository orderHistoryItemRepository) : IOrderService
{
    public async Task<Order> CreateOrderAsync(string createdBy, CancellationToken ct)
    {
        using TransactionScope transaction = CreateTransactionScope();

        OrderCreationParameters orderParameters = new(
            state: OrderState.Created,
            createdAt: DateTime.UtcNow,
            createdBy: createdBy);

        Order order = await orderRepository.CreateOrderAsync(orderParameters, ct);

        OrderHistoryItemCreationParameters orderHistoryItemParameters = new(
            orderId: order.Id,
            createdAt: DateTime.UtcNow,
            kind: OrderHistoryItemKind.Created,
            payload: new OrderCreated());

        await orderHistoryItemRepository.CreateOrderHistoryItemAsync(orderHistoryItemParameters, ct);

        transaction.Complete();
        return order;
    }

    public async Task<OrderItem> AddOrderItemAsync(
        OrderId orderId,
        ProductId productId,
        int quantity,
        CancellationToken ct)
    {
        using TransactionScope transaction = CreateTransactionScope();

        Order order = await orderRepository.FindOrderByIdAsync(orderId, ct)
                      ?? throw new InvalidOperationException("Order not found");

        if (order.State != OrderState.Created)
            throw new InvalidOperationException("can be deleted when order is in created");

        OrderItemCreationParameters orderItemParameters = new(
            orderId: orderId,
            productId: productId,
            quantity: quantity,
            isDeleted: false);

        OrderItem orderItem = await orderItemRepository.CreateOrderItemAsync(orderItemParameters, ct);

        OrderHistoryItemCreationParameters dtoHistoryItem = new(
            orderId: order.Id,
            createdAt: DateTime.UtcNow,
            kind: OrderHistoryItemKind.ItemAdded,
            payload: new ItemAdded(orderItem.Id));

        await orderHistoryItemRepository.CreateOrderHistoryItemAsync(dtoHistoryItem, ct);

        transaction.Complete();
        return orderItem;
    }

    public async Task RemoveOrderItemAsync(OrderItemId orderItemId, CancellationToken ct)
    {
        using TransactionScope transaction = CreateTransactionScope();

        OrderItem orderItem = await orderItemRepository.FindOrderItemByIdAsync(orderItemId, ct)
                              ?? throw new InvalidOperationException("Item not found");

        Order order = await orderRepository.FindOrderByIdAsync(orderItem.OrderId, ct)
                      ?? throw new InvalidOperationException("Order not found");

        if (order.State != OrderState.Created)
            throw new InvalidOperationException("can be deleted when order is in created");

        await orderItemRepository.SoftDeleteOrderItemAsync(orderItem, ct);

        OrderHistoryItemCreationParameters dtoHistoryItem = new(
            orderId: order.Id,
            createdAt: DateTime.UtcNow,
            kind: OrderHistoryItemKind.ItemRemoved,
            payload: new ItemRemoved(orderItemId));

        await orderHistoryItemRepository.CreateOrderHistoryItemAsync(dtoHistoryItem, ct);

        transaction.Complete();
    }

    public async Task SetProcessingAsync(OrderId orderId, CancellationToken ct)
    {
        await ChangeStatus(orderId, OrderState.Processing, ct);
    }

    public async Task SetCompletedAsync(OrderId orderId, CancellationToken ct)
    {
        await ChangeStatus(orderId, OrderState.Completed, ct);
    }

    public async Task SetCancelledAsync(OrderId orderId, CancellationToken ct)
    {
        using TransactionScope transaction = CreateTransactionScope();

        Order order = await orderRepository.FindOrderByIdAsync(orderId, ct)
                      ?? throw new InvalidOperationException("Order not found");

        if (order.State is not OrderState.Created)
        {
            throw new InvalidOperationException("can be deleted when order is in created");
        }

        OrderState previous = order.State;

        Order orderCurrent = new(
            id: order.Id,
            state: OrderState.Cancelled,
            createdAt: order.CreatedAt,
            createdBy: order.CreatedBy);

        await orderRepository.ChangeStatusAsync(orderCurrent, ct);

        OrderHistoryItemCreationParameters dtoHistoryItem = new(
            orderId: order.Id,
            createdAt: DateTime.UtcNow,
            kind: OrderHistoryItemKind.StateChanged,
            payload: new StateChanged(previous, OrderState.Cancelled));

        await orderHistoryItemRepository.CreateOrderHistoryItemAsync(dtoHistoryItem, ct);

        transaction.Complete();
    }

    public async Task ReportHistoryEventApprovedAsync(OrderId orderId, CancellationToken ct)
    {
        await WriteProcessingStageInHistory(orderId, ProcessingStage.Approved, ct);
    }

    public async Task ReportHistoryEventPackingAsync(OrderId orderId, CancellationToken ct)
    {
        await WriteProcessingStageInHistory(orderId, ProcessingStage.Packing, ct);
    }

    public async Task ReportHistoryEventPackedAsync(OrderId orderId, CancellationToken ct)
    {
        await WriteProcessingStageInHistory(orderId, ProcessingStage.Packed, ct);
    }

    public async Task ReportHistoryEventInDeliveryAsync(OrderId orderId, CancellationToken ct)
    {
        await WriteProcessingStageInHistory(orderId, ProcessingStage.InDelivery, ct);
    }

    public async Task ReportHistoryEventDeliveredAsync(OrderId orderId, CancellationToken ct)
    {
        await WriteProcessingStageInHistory(orderId, ProcessingStage.Delivered, ct);
    }

    public async IAsyncEnumerable<OrderHistoryItem> FindOrderHistoryItemsAsync(
        OrderId orderId,
        PaginationInfo<OrderHistoryItemId> paginationInfo,
        [EnumeratorCancellation] CancellationToken ct)
    {
        IAsyncEnumerable<OrderHistoryItem> stream = orderHistoryItemRepository.FindOrderHistoryItemsAsync(
            new OrderHistoryItemSearchingFilters
            {
                OrderIds = [orderId],
            },
            paginationInfo,
            ct);

        await foreach (OrderHistoryItem item in stream)
        {
            yield return item;
        }
    }

    private static TransactionScope CreateTransactionScope()
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
    }

    private async Task WriteProcessingStageInHistory(OrderId orderId, ProcessingStage processingStage, CancellationToken ct)
    {
        OrderHistoryItemCreationParameters dtoHistoryItem = new(
            orderId: orderId,
            createdAt: DateTime.UtcNow,
            kind: OrderHistoryItemKind.StateChanged,
            payload: new ProcessingStageChanged(processingStage));

        await orderHistoryItemRepository.CreateOrderHistoryItemAsync(dtoHistoryItem, ct);
    }

    private async Task ChangeStatus(
        OrderId orderId,
        OrderState current,
        CancellationToken ct)
    {
        using TransactionScope transaction = CreateTransactionScope();

        Order order = await orderRepository.FindOrderByIdAsync(orderId, ct)
                      ?? throw new InvalidOperationException("Order not found");

        OrderState previous = order.State;

        Order orderCurrent = new(
            id: order.Id,
            state: current,
            createdAt: order.CreatedAt,
            createdBy: order.CreatedBy);

        await orderRepository.ChangeStatusAsync(orderCurrent, ct);

        OrderHistoryItemCreationParameters dtoHistoryItem = new(
            orderId: order.Id,
            createdAt: DateTime.UtcNow,
            kind: OrderHistoryItemKind.StateChanged,
            payload: new StateChanged(previous, current));

        await orderHistoryItemRepository.CreateOrderHistoryItemAsync(dtoHistoryItem, ct);

        transaction.Complete();
    }
}