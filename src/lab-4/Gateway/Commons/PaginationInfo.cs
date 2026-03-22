using Microsoft.AspNetCore.Mvc;

namespace Gateway.Commons;

public sealed record PaginationInfo
{
    [FromQuery(Name = "pageSize")]
    public required int PageSize { get; init; }

    [FromQuery(Name = "pageToken")]
    public long? PageToken { get; init; } = null;
}