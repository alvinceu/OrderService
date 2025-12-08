using Application.Models.Primitives.EntityIds;
using Google.Protobuf.WellKnownTypes;
using Lab3.Contracts.Services.Order;
using System.Diagnostics;
using DomainHistoryEvent = Application.Models.Primitives.CompositeObjects.OrderHistoryEvent;
using DomainHistoryKind = Application.Models.Primitives.Enums.OrderHistoryItemKind;
using DomainItemAdded = Application.Models.Primitives.CompositeObjects.ItemAdded;
using DomainItemRemoved = Application.Models.Primitives.CompositeObjects.ItemRemoved;
using DomainOrder = Application.Models.Entities.Order;
using DomainOrderCreated = Application.Models.Primitives.CompositeObjects.OrderCreated;
using DomainOrderHistoryItem = Application.Models.Entities.OrderHistoryItem;
using DomainOrderItem = Application.Models.Entities.OrderItem;
using DomainOrderState = Application.Models.Primitives.Enums.OrderState;
using DomainPaginationOrderHistoryItemId = Application.Models.Commons.Paginations.PaginationInfo<Application.Models.Primitives.EntityIds.OrderHistoryItemId>;
using DomainProcessingStage = Application.Models.Primitives.Enums.ProcessingStage;
using DomainProcessingStageChanged = Application.Models.Primitives.CompositeObjects.ProcessingStageChanged;
using DomainStateChanged = Application.Models.Primitives.CompositeObjects.StateChanged;
using GrpcOrder = Lab3.Contracts.Services.Order.Order;
using GrpcOrderHistoryItem = Lab3.Contracts.Services.Order.OrderHistoryItem;
using GrpcPaginationOrderHistoryItem = Lab3.Contracts.Services.Order.PaginationInfo;
using GrpcProcessingStage = Lab3.Contracts.Services.Order.ProcessingStage;

namespace Presentation.Grpc.Mappers;

internal static class OrderMapper
{
    public static AddOrderItemResponse ToGrpc(this DomainOrderItem item)
    {
        return new AddOrderItemResponse
        {
            Item = new OrderItem
            {
                OrderItemId = item.Id.Value,
                OrderId = item.OrderId.Value,
                ProductId = item.ProductId.Value,
                Quantity = item.Quantity,
                IsDeleted = item.IsDeleted,
            },
        };
    }

    public static GrpcProcessingStage ToGrpc(this DomainProcessingStage stage)
    {
        return stage switch
        {
            DomainProcessingStage.Approved => GrpcProcessingStage.Approved,
            DomainProcessingStage.Packing => GrpcProcessingStage.Packing,
            DomainProcessingStage.Packed => GrpcProcessingStage.Packed,
            DomainProcessingStage.InDelivery => GrpcProcessingStage.InDelivery,
            DomainProcessingStage.Delivered => GrpcProcessingStage.Delivered,
            _ => throw new UnreachableException(),
        };
    }

    public static CreateOrderResponse ToGrpc(this DomainOrder order)
    {
        return new CreateOrderResponse
        {
            Order = new GrpcOrder
            {
                OrderId = order.Id.Value,
                OrderState = order.State.ToGrpc(),
                CreatedAt = order.CreatedAt.ToTimestamp(),
                CreatedBy = order.CreatedBy,
            },
        };
    }

    public static FindOrderHistoryResponse ToGrpc(this DomainOrderHistoryItem item)
    {
        return new FindOrderHistoryResponse
        {
            Item = new GrpcOrderHistoryItem
            {
                OrderHistoryItemId = item.Id.Value,
                OrderId = item.OrderId.Value,
                CreatedAt = item.CreatedAt.ToTimestamp(),
                Kind = item.Kind.ToGrpc(),
                Payload = item.Payload.ToGrpc(),
            },
        };
    }

    public static OrderState ToGrpc(this DomainOrderState state)
    {
        return state switch
        {
            DomainOrderState.Created => OrderState.Created,
            DomainOrderState.Processing => OrderState.Processing,
            DomainOrderState.Completed => OrderState.Completed,
            DomainOrderState.Cancelled => OrderState.Cancelled,
            _ => throw new UnreachableException(),
        };
    }

    public static OrderHistoryItemKind ToGrpc(this DomainHistoryKind kind)
    {
        return kind switch
        {
            DomainHistoryKind.Created => OrderHistoryItemKind.Created,
            DomainHistoryKind.ItemAdded => OrderHistoryItemKind.ItemAdded,
            DomainHistoryKind.ItemRemoved => OrderHistoryItemKind.ItemRemoved,
            DomainHistoryKind.StateChanged => OrderHistoryItemKind.StateChanged,
            _ => throw new UnreachableException(),
        };
    }

    public static OrderHistoryItemPayload ToGrpc(this DomainHistoryEvent payload)
    {
        return payload switch
        {
            DomainItemAdded added =>
                new OrderHistoryItemPayload
                {
                    ItemAdded = new ItemAddedPayload
                    {
                        OrderItemId = added.ItemId.Value,
                    },
                },

            DomainItemRemoved removed =>
                new OrderHistoryItemPayload
                {
                    ItemRemoved = new ItemRemovedPayload
                    {
                        OrderItemId = removed.ItemId.Value,
                    },
                },

            DomainOrderCreated =>
                new OrderHistoryItemPayload
                {
                    OrderCreated = new OrderCreatedPayload(),
                },

            DomainStateChanged changed =>
                new OrderHistoryItemPayload
                {
                    StateChanged = new StateChangedPayload
                    {
                        PreviousState = changed.Previous.ToGrpc(),
                        CurrentState = changed.Current.ToGrpc(),
                    },
                },
            DomainProcessingStageChanged changed =>
                new OrderHistoryItemPayload
                {
                    ProcessingStateChanged = new ProcessingStageChangedPayload
                    {
                        Stage = changed.Stage.ToGrpc(),
                    },
                },

            _ => throw new UnreachableException(),
        };
    }

    public static DomainPaginationOrderHistoryItemId ToDomain(this GrpcPaginationOrderHistoryItem grpc)
    {
        return grpc.HasPageToken
            ? new DomainPaginationOrderHistoryItemId(grpc.PageSize, OrderHistoryItemId.Create(grpc.PageToken))
            : new DomainPaginationOrderHistoryItemId(grpc.PageSize);
    }
}