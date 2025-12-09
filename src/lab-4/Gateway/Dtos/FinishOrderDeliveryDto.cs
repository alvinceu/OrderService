namespace Gateway.Dtos;

public sealed record FinishOrderDeliveryDto
{
    public required bool IsSuccessful { get; init; }

    public string? FailureReason { get; init; }
}