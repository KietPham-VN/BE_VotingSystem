namespace BE_VotingSystem.Application.Dtos.Lecture.Requests;

/// <summary>
///     Request parameters for retrieving current user's vote history
/// </summary>
public sealed record GetVoteHistoryRequest(int Page = 1, int PageSize = 20);
