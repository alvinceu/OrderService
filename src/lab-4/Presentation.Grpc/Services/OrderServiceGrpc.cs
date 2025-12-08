using Application.Contracts.Services;
using Application.Models.Primitives.EntityIds;
using Grpc.Core;
using Lab3.Contracts.Services.Order;
using Presentation.Grpc.Commons;
using Presentation.Grpc.Mappers;
using DomainOrder = Application.Models.Entities.Order;
using OrderHistoryItem = Application.Models.Entities.OrderHistoryItem;
using OrderItem = Application.Models.Entities.OrderItem;

namespace Presentation.Grpc.Services;

internal sealed class OrderServiceGrpc : OrderService.OrderServiceBase
{
    private readonly IOrderService _orderService;
    private readonly IRequestValidator<AddOrderItemRequest> _addOrderItemRequestValidator;
    private readonly IRequestValidator<CreateOrderRequest> _createOrderRequestValidator;
    private readonly IRequestValidator<FindOrderHistoryItemsRequest> _getOrderHistoryItemsRequestValidator;
    private readonly IRequestValidator<RemoveOrderItemRequest> _removeOrderItemRequestValidator;
    private readonly IRequestValidator<SetOrderStateProcessingRequest> _setOrderStateProcessingRequestValidator;
    private readonly IRequestValidator<SetOrderStateCancelledRequest> _setOrderStateCancelledRequestValidator;

    public OrderServiceGrpc(
        IOrderService orderService,
        IRequestValidator<AddOrderItemRequest> addOrderItemRequestValidator,
        IRequestValidator<CreateOrderRequest> createOrderRequestValidator,
        IRequestValidator<FindOrderHistoryItemsRequest> getOrderHistoryItemsRequestValidator,
        IRequestValidator<RemoveOrderItemRequest> removeOrderItemRequestValidator,
        IRequestValidator<SetOrderStateProcessingRequest> setOrderStateProcessingRequestValidator,
        IRequestValidator<SetOrderStateCancelledRequest> setOrderStateCancelledRequestValidator)
    {
        _orderService = orderService;
        _addOrderItemRequestValidator = addOrderItemRequestValidator;
        _createOrderRequestValidator = createOrderRequestValidator;
        _getOrderHistoryItemsRequestValidator = getOrderHistoryItemsRequestValidator;
        _removeOrderItemRequestValidator = removeOrderItemRequestValidator;
        _setOrderStateProcessingRequestValidator = setOrderStateProcessingRequestValidator;
        _setOrderStateCancelledRequestValidator = setOrderStateCancelledRequestValidator;
    }

    public override async Task<AddOrderItemResponse> AddOrderItem(AddOrderItemRequest request, ServerCallContext context)
    {
        _addOrderItemRequestValidator.Validate(request);

        OrderItem orderItem = await _orderService.AddOrderItemAsync(
            OrderId.Create(request.OrderId),
            ProductId.Create(request.ProductId),
            request.Quantity,
            context.CancellationToken);

        return orderItem.ToGrpc();
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        _createOrderRequestValidator.Validate(request);

        DomainOrder order = await _orderService.CreateOrderAsync(request.CreatedBy, context.CancellationToken);

        return order.ToGrpc();
    }

    public override async Task FindOrderHistoryItems(
        FindOrderHistoryItemsRequest request,
        IServerStreamWriter<FindOrderHistoryResponse> writer,
        ServerCallContext context)
    {
        _getOrderHistoryItemsRequestValidator.Validate(request);

        IAsyncEnumerable<OrderHistoryItem> stream =
            _orderService.FindOrderHistoryItemsAsync(
                OrderId.Create(request.OrderId),
                request.PaginationInfo.ToDomain(),
                context.CancellationToken);

        await foreach (OrderHistoryItem item in stream)
        {
            await writer.WriteAsync(item.ToGrpc());
        }
    }

    public override async Task<RemoveOrderItemResponse> RemoveOrderItem(RemoveOrderItemRequest request, ServerCallContext context)
    {
        _removeOrderItemRequestValidator.Validate(request);

        await _orderService.RemoveOrderItemAsync(OrderItemId.Create(request.OrderItemId), context.CancellationToken);

        return new RemoveOrderItemResponse();
    }

    public override async Task<SetOrderStateCancelledResponse> SetCancelled(SetOrderStateCancelledRequest request, ServerCallContext context)
    {
        _setOrderStateCancelledRequestValidator.Validate(request);

        await _orderService.SetCancelledOrderInCreatedStateAsync(OrderId.Create(request.OrderId), context.CancellationToken);

        return new SetOrderStateCancelledResponse();
    }

    public override async Task<SetOrderStateProcessingResponse> SetProcessing(SetOrderStateProcessingRequest request, ServerCallContext context)
    {
        _setOrderStateProcessingRequestValidator.Validate(request);

        await _orderService.SetProcessingAsync(OrderId.Create(request.OrderId), context.CancellationToken);

        return new SetOrderStateProcessingResponse();
    }
}