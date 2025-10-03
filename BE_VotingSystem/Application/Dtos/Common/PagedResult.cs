namespace BE_VotingSystem.Application.Dtos.Common;

/// <summary>
///     Standard paged result wrapper for list endpoints
/// </summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / Math.Max(1, PageSize));
}
