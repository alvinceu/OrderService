using Gateway.Dtos;
using Orders.ProcessingService.Contracts;

namespace Gateway.Mappers;

public static class OrderProcessingMapper
{
    public static ApproveOrderRequest ToGrpc(this ApproveOrderDto dto, long orderId)
    {
        return new ApproveOrderRequest
        {
            OrderId = orderId,
            ApprovedBy = dto.ApprovedBy,
            IsApproved = dto.IsApproved,
            FailureReason = dto.FailedReason,
        };
    }

    public static StartOrderPackingRequest ToGrpc(this StartOrderPackingDto dto, long orderId)
    {
        return new StartOrderPackingRequest
        {
            OrderId = orderId,
            PackingBy = dto.PackingBy,
        };
    }

    public static FinishOrderPackingRequest ToGrpc(this FinishOrderPackingDto dto, long orderId)
    {
        return new FinishOrderPackingRequest
        {
            OrderId = orderId,
            IsSuccessful = dto.IsSuccessful,
            FailureReason = dto.FailureReason,
        };
    }

    public static StartOrderDeliveryRequest ToGrpc(this StartOrderDeliveryDto dto, long orderId)
    {
        return new StartOrderDeliveryRequest
        {
            OrderId = orderId,
            DeliveredBy = dto.DeliveredBy,
        };
    }

    public static FinishOrderDeliveryRequest ToGrpc(this FinishOrderDeliveryDto dto, long orderId)
    {
        return new FinishOrderDeliveryRequest
        {
            OrderId = orderId,
            IsSuccessful = dto.IsSuccessful,
            FailureReason = dto.FailureReason,
        };
    }
}