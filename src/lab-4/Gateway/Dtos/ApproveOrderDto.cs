namespace Gateway.Dtos;

// Dto
public sealed record ApproveOrderDto
{
    public required bool IsApproved { get; init; }

    public required string ApprovedBy { get; init; }

    public string? FailedReason { get; init; }
}