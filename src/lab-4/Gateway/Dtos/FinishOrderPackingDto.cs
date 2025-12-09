namespace Gateway.Dtos;

public sealed record FinishOrderPackingDto
{
    public required bool IsSuccessful { get; init; }

    public string? FailureReason { get; init; }
}