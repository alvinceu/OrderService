using Gateway.Primitives.Enums;

namespace Gateway.Dtos;

public sealed record OrderChangeStatusDto
{
    public required OrderChangeStatusState StatusState { get; init; }
}