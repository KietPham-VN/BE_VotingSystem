using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Application.Dtos.Common;

namespace BE_VotingSystem.Application.Interfaces.Services;

/// <summary>
///     Service interface for lecturer voting
/// </summary>
public interface ILectureVoteService
{
    /// <summary>
    ///     Get today's votes by a specific lecturer
    /// </summary>
    Task<IReadOnlyList<VoteDto>> GetVotesByLectureAsync(Guid lectureId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get all-time votes by a specific lecturer (no date filter)
    /// </summary>
    Task<IReadOnlyList<VoteDto>> GetAllVotesByLectureAsync(Guid lectureId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Cast a vote for a lecturer by the current account
    /// </summary>
    Task<bool> VoteAsync(Guid accountId, CreateLectureVoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Cancel today's vote for a lecturer by the current account
    /// </summary>
    Task<bool> CancelAsync(Guid accountId, CancelLectureVoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get current user's lecturer vote history with pagination
    /// </summary>
    Task<PagedResult<VoteHistoryItemDto>> GetMyVoteHistoryAsync(Guid accountId, int page, int pageSize, CancellationToken cancellationToken = default);
}