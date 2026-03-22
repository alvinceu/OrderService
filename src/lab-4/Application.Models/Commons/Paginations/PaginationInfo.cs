namespace Application.Models.Commons.Paginations;

public sealed record PaginationInfo<TId> where TId : struct, IId<TId>, IEquatable<TId>
{
    public int PageSize { get; }

    public TId? PageToken { get; }

    public PaginationInfo(int pageSize, TId? pageToken = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        PageSize = pageSize;
        PageToken = pageToken;
    }
}