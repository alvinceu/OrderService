using Application.Abstractions.Services;
using Application.Models.Commons.Paginations;
using Application.Models.Entities;
using Application.Models.Primitives.EntityIds;
using Confluent.Kafka;
using Integration.Kafka.Commons;
using Orders.Kafka.Contracts;
using System.Runtime.CompilerServices;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

namespace Presentation.Kafka.Services;

internal sealed class WrappedOrderService : IOrderService
{
    private readonly IOrderService _innerOrderService;

    private readonly IKafkaMessageProducer<OrderCreationKey, OrderCreationValue> _producer;

    public WrappedOrderService(IOrderService innerOrderService, IKafkaMessageProducer<OrderCreationKey, OrderCreationValue> producer)
    {
        _innerOrderService = innerOrderService;
        _producer = producer;
    }

    public async Task<Order> CreateOrderAsync(string createdBy, CancellationToken ct)
    {
        Order result = await _innerOrderService.CreateOrderAsync(createdBy, ct);

        var key = new OrderCreationKey
        {
            OrderId = result.Id.Value,
        };

        var value = new OrderCreationValue
        {
            OrderCreated = new OrderCreationValue.Types.OrderCreated
            {
                OrderId = result.Id.Value,
                CreatedAt = Timestamp.FromDateTime(result.CreatedAt),
            },
        };

        var message = new Message<OrderCreationKey, OrderCreationValue>
        {
            Key = key,
            Value = value,
        };

        await _producer.ProduceAsync(message, ct);

        return result;
    }

    public async Task<OrderItem> AddOrderItemAsync(OrderId orderId, ProductId productId, int quantity, CancellationToken ct)
    {
        return await _innerOrderService.AddOrderItemAsync(orderId, productId, quantity, ct);
    }

    public async Task SetCancelledOrderInCreatedStateAsync(OrderId orderId, CancellationToken ct)
    {
        await _innerOrderService.SetCancelledOrderInCreatedStateAsync(orderId, ct);
    }

    public async Task RemoveOrderItemAsync(OrderItemId orderItemId, CancellationToken ct)
    {
        await _innerOrderService.RemoveOrderItemAsync(orderItemId, ct);
    }

    public async Task SetProcessingAsync(OrderId orderId, CancellationToken ct)
    {
        await _innerOrderService.SetProcessingAsync(orderId, ct);

        var key = new OrderCreationKey
        {
            OrderId = orderId.Value,
        };

        var value = new OrderCreationValue
        {
            OrderProcessingStarted = new OrderCreationValue.Types.OrderProcessingStarted
            {
                OrderId = orderId.Value,
                StartedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            },
        };

        var message = new Message<OrderCreationKey, OrderCreationValue>
        {
            Key = key,
            Value = value,
        };

        await _producer.ProduceAsync(message, ct);
    }

    public async Task SetCompletedAsync(OrderId orderId, CancellationToken ct)
    {
        await _innerOrderService.SetCompletedAsync(orderId, ct);
    }

    public async Task SetCancelledAsync(OrderId orderId, CancellationToken ct)
    {
        await _innerOrderService.SetCancelledAsync(orderId, ct);
    }

    public async Task ReportHistoryEventApprovedAsync(OrderId orderId, CancellationToken ct)
    {
        await _innerOrderService.ReportHistoryEventApprovedAsync(orderId, ct);
    }

    public async Task ReportHistoryEventPackingAsync(OrderId orderId, CancellationToken ct)
    {
        await _innerOrderService.ReportHistoryEventPackingAsync(orderId, ct);
    }

    public async Task ReportHistoryEventPackedAsync(OrderId orderId, CancellationToken ct)
    {
        await _innerOrderService.ReportHistoryEventPackedAsync(orderId, ct);
    }

    public async Task ReportHistoryEventInDeliveryAsync(OrderId orderId, CancellationToken ct)
    {
        await _innerOrderService.ReportHistoryEventInDeliveryAsync(orderId, ct);
    }

    public async Task ReportHistoryEventDeliveredAsync(OrderId orderId, CancellationToken ct)
    {
        await _innerOrderService.ReportHistoryEventDeliveredAsync(orderId, ct);
    }

    public async IAsyncEnumerable<OrderHistoryItem> FindOrderHistoryItemsAsync(
        OrderId orderId,
        PaginationInfo<OrderHistoryItemId> paginationInfo,
        [EnumeratorCancellation] CancellationToken ct)
    {
        await foreach (OrderHistoryItem item in _innerOrderService.FindOrderHistoryItemsAsync(orderId, paginationInfo, ct))
        {
            yield return item;
        }
    }
}