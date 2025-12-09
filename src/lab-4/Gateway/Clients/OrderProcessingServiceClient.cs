using Gateway.Dtos;
using Gateway.Mappers;
using Orders.ProcessingService.Contracts;

namespace Gateway.Clients;

public sealed class OrderProcessingServiceClient(OrderService.OrderServiceClient grpcClient)
{
    public async Task ApproveOrderAsync(long orderId, ApproveOrderDto dto, CancellationToken cancellationToken)
    {
        ApproveOrderRequest request = dto.ToGrpc(orderId);

        await grpcClient.ApproveOrderAsync(request, cancellationToken: cancellationToken);
    }

    public async Task StartOrderPackingAsync(long orderId, StartOrderPackingDto dto, CancellationToken cancellationToken)
    {
        StartOrderPackingRequest request = dto.ToGrpc(orderId);

        await grpcClient.StartOrderPackingAsync(request, cancellationToken: cancellationToken);
    }

    public async Task FinishOrderPackingAsync(long orderId, FinishOrderPackingDto dto, CancellationToken cancellationToken)
    {
        FinishOrderPackingRequest request = dto.ToGrpc(orderId);

        await grpcClient.FinishOrderPackingAsync(request, cancellationToken: cancellationToken);
    }

    public async Task StartOrderDeliveryAsync(long orderId, StartOrderDeliveryDto dto, CancellationToken cancellationToken)
    {
        StartOrderDeliveryRequest request = dto.ToGrpc(orderId);

        await grpcClient.StartOrderDeliveryAsync(request, cancellationToken: cancellationToken);
    }

    public async Task FinishOrderDeliveryAsync(long orderId, FinishOrderDeliveryDto dto, CancellationToken cancellationToken)
    {
        FinishOrderDeliveryRequest request = dto.ToGrpc(orderId);

        await grpcClient.FinishOrderDeliveryAsync(request, cancellationToken: cancellationToken);
    }
}