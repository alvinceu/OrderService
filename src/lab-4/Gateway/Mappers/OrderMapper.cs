using Gateway.Dtos;
using Gateway.Primitives.CompositeObjects;
using Gateway.Primitives.Enums;
using Lab3.Contracts.Services.Order;
using System.Diagnostics;
using AddOrderItemResponseGrpc = Lab3.Contracts.Services.Order.AddOrderItemResponse;
using CreateOrderResponseGrpc = Lab3.Contracts.Services.Order.CreateOrderResponse;
using FindOrderHistoryResponseGrpc = Lab3.Contracts.Services.Order.FindOrderHistoryResponse;
using GrpcHistoryKind = Lab3.Contracts.Services.Order.OrderHistoryItemKind;
using GrpcOrderState = Lab3.Contracts.Services.Order.OrderState;
using GrpcPaginationInfo = Lab3.Contracts.Services.Order.PaginationInfo;
using GrpcPayload = Lab3.Contracts.Services.Order.OrderHistoryItemPayload;
using GrpcProcessingStage = Lab3.Contracts.Services.Order.ProcessingStage;
using PaginationInfo = Gateway.Commons.PaginationInfo;

namespace Gateway.Mappers;

internal static class OrderMapper
{
    public static CreateOrderRequest ToGrpc(this CreateOrderDto dto)
    {
        return new CreateOrderRequest
        {
            CreatedBy = dto.CreatedBy,
        };
    }

    public static OrderCreatedDto ToDto(this CreateOrderResponseGrpc grpc)
    {
        return new OrderCreatedDto
        {
            OrderId = grpc.Order.OrderId,
            State = grpc.Order.OrderState.ToDto(),
            CreatedAt = grpc.Order.CreatedAt.ToDateTime(),
            CreatedBy = grpc.Order.CreatedBy,
        };
    }

    public static OrderProcessingStage ToDto(this GrpcProcessingStage grpc)
    {
        return grpc switch
        {
            GrpcProcessingStage.Approved => OrderProcessingStage.Approved,
            GrpcProcessingStage.Packing => OrderProcessingStage.Packing,
            GrpcProcessingStage.Packed => OrderProcessingStage.Packed,
            GrpcProcessingStage.InDelivery => OrderProcessingStage.InDelivery,
            GrpcProcessingStage.Delivered => OrderProcessingStage.Delivered,
            _ => throw new UnreachableException(),
        };
    }

    public static AddOrderItemRequest ToGrpc(this AddOrderItemDto dto, long orderId)
    {
        return new AddOrderItemRequest
        {
            OrderId = orderId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
        };
    }

    public static OrderItemAddedDto ToDto(this AddOrderItemResponseGrpc grpc)
    {
        return new OrderItemAddedDto
        {
            OrderItemId = grpc.Item.OrderItemId,
            OrderId = grpc.Item.OrderId,
            ProductId = grpc.Item.ProductId,
            Quantity = grpc.Item.Quantity,
            IsDeleted = grpc.Item.IsDeleted,
        };
    }

    public static RemoveOrderItemRequest ToRemoveOrderItemGrpc(long orderItemId)
    {
        return new RemoveOrderItemRequest
        {
            OrderItemId = orderItemId,
        };
    }

    public static SetOrderStateProcessingRequest ToSetOrderStateProcessingGrpc(long orderId)
    {
        return new SetOrderStateProcessingRequest
        {
            OrderId = orderId,
        };
    }

    public static SetOrderStateCancelledRequest ToSetOrderStateCancelledGrpc(long orderId)
    {
        return new SetOrderStateCancelledRequest
        {
            OrderId = orderId,
        };
    }

    public static FindOrderHistoryItemsRequest ToGrpc(long orderId, PaginationInfo info)
    {
        return new FindOrderHistoryItemsRequest
        {
            OrderId = orderId,
            PaginationInfo = info.PageToken is { } token
                ? new GrpcPaginationInfo { PageSize = info.PageSize, PageToken = token }
                : new GrpcPaginationInfo { PageSize = info.PageSize },
        };
    }

    public static OrderHistoryItemDto ToDto(this FindOrderHistoryResponseGrpc grpc)
    {
        return new OrderHistoryItemDto
        {
            OrderHistoryItemId = grpc.Item.OrderHistoryItemId,
            OrderId = grpc.Item.OrderId,
            CreatedAt = grpc.Item.CreatedAt.ToDateTime(),
            Kind = grpc.Item.Kind.ToDto(),
            Payload = grpc.Item.Payload.ToDto(),
        };
    }

    public static string ToDto(this GrpcOrderState state)
    {
        return state switch
        {
            GrpcOrderState.Created => "Created",
            GrpcOrderState.Processing => "Processing",
            GrpcOrderState.Completed => "Completed",
            GrpcOrderState.Cancelled => "Cancelled",
            _ => throw new UnreachableException(),
        };
    }

    public static string ToDto(this GrpcHistoryKind kind)
    {
        return kind switch
        {
            GrpcHistoryKind.Created => "Created",
            GrpcHistoryKind.ItemAdded => "ItemAdded",
            GrpcHistoryKind.ItemRemoved => "ItemRemoved",
            GrpcHistoryKind.StateChanged => "StateChanged",
            _ => throw new UnreachableException(),
        };
    }

    public static OrderHistoryEvent ToDto(this GrpcPayload payload)
    {
        return payload.PayloadCase switch
        {
            GrpcPayload.PayloadOneofCase.None =>
                throw new UnreachableException(),

            GrpcPayload.PayloadOneofCase.OrderCreated =>
                new OrderCreated(),

            GrpcPayload.PayloadOneofCase.ItemAdded =>
                new ItemAdded
                {
                    OrderItemId = payload.ItemAdded.OrderItemId,
                },
            GrpcPayload.PayloadOneofCase.ItemRemoved =>
                new ItemRemoved
                {
                    OrderItemId = payload.ItemRemoved.OrderItemId,
                },
            GrpcPayload.PayloadOneofCase.StateChanged =>
                new StateChanged
                {
                    PreviousState = payload.StateChanged.PreviousState.ToDto(),
                    CurrentState = payload.StateChanged.CurrentState.ToDto(),
                },
            GrpcPayload.PayloadOneofCase.ProcessingStateChanged =>
                new ProcessingStageChanged
                {
                    Stage = payload.ProcessingStateChanged.Stage.ToDto(),
                },

            _ => throw new UnreachableException(),
        };
    }
}