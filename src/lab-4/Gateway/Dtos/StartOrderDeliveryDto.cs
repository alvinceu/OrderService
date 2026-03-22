namespace Gateway.Dtos;

public sealed record StartOrderDeliveryDto
{
    public required string DeliveredBy { get; init; }
}