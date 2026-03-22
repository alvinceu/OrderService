using Gateway.Dtos;
using Gateway.Mappers;
using Gateway.Primitives.Enums;
using Grpc.Core;
using Lab3.Contracts.Services.Order;
using System.Runtime.CompilerServices;

namespace Gateway.Clients;

public sealed class OrderServiceClient(OrderService.OrderServiceClient grpcClient)
{
    public async Task<OrderCreatedDto> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct)
    {
        CreateOrderRequest request = dto.ToGrpc();

        CreateOrderResponse grpcOrder = await grpcClient.CreateOrderAsync(request, cancellationToken: ct);

        OrderCreatedDto response = grpcOrder.ToDto();

        return response;
    }

    public async Task<OrderItemAddedDto> AddOrderItemAsync(long orderId, AddOrderItemDto dto, CancellationToken ct)
    {
        AddOrderItemRequest request = dto.ToGrpc(orderId);

        AddOrderItemResponse grpcOrderItem = await grpcClient.AddOrderItemAsync(request, cancellationToken: ct);

        OrderItemAddedDto response = grpcOrderItem.ToDto();

        return response;
    }

    public async Task RemoveOrderItemAsync(long orderItemId, CancellationToken ct)
    {
        RemoveOrderItemRequest request = OrderMapper.ToRemoveOrderItemGrpc(orderItemId);

        await grpcClient.RemoveOrderItemAsync(request, cancellationToken: ct);
    }

    public async IAsyncEnumerable<OrderHistoryItemDto> FindOrderHistoryItemsAsync(
        long orderId,
        Commons.PaginationInfo info,
        [EnumeratorCancellation] CancellationToken ct)
    {
        FindOrderHistoryItemsRequest request = OrderMapper.ToGrpc(orderId, info);

        using AsyncServerStreamingCall<FindOrderHistoryResponse> call =
             grpcClient.FindOrderHistoryItems(request, cancellationToken: ct);

        await foreach (FindOrderHistoryResponse item in call.ResponseStream.ReadAllAsync(cancellationToken: ct))
        {
            yield return item.ToDto();
        }
    }

    public async Task OrderChangeStatusAsync(long orderId, OrderChangeStatusDto dto, CancellationToken ct)
    {
        if (dto.StatusState == OrderChangeStatusState.Processing)
        {
            await SetProcessingAsync(orderId, ct);
        }

        if (dto.StatusState == OrderChangeStatusState.Cancelled)
        {
            await SetCancelledAsync(orderId, ct);
        }
    }

    private async Task SetProcessingAsync(long orderId, CancellationToken ct)
    {
        SetOrderStateProcessingRequest processingRequest = OrderMapper.ToSetOrderStateProcessingGrpc(orderId);

        await grpcClient.SetProcessingAsync(processingRequest, cancellationToken: ct);
    }

    private async Task SetCancelledAsync(long orderId, CancellationToken ct)
    {
        SetOrderStateCancelledRequest cancelledRequest = OrderMapper.ToSetOrderStateCancelledGrpc(orderId);

        await grpcClient.SetCancelledAsync(cancelledRequest, cancellationToken: ct);
    }
}