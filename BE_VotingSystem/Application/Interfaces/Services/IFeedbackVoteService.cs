using BE_VotingSystem.Application.Dtos.FeedbackVote;
using BE_VotingSystem.Application.Dtos.FeedbackVote.Requests;

namespace BE_VotingSystem.Application.Interfaces.Services;

/// <summary>
///     Service interface for managing website feedback votes
/// </summary>
public interface IFeedbackVoteService
{
    /// <summary>
    ///     Create a feedback vote for the specified account. Each account can only have one vote.
    /// </summary>
    Task<FeedbackVoteDto> CreateAsync(Guid accountId, CreateFeedbackVoteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update the feedback vote for the specified account
    /// </summary>
    Task<FeedbackVoteDto> UpdateAsync(Guid accountId, UpdateFeedbackVoteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get current feedback vote for the specified account, or null if not voted yet
    /// </summary>
    Task<FeedbackVoteDto?> GetAsync(Guid accountId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get all website feedback votes
    /// </summary>
    Task<IReadOnlyList<FeedbackVoteDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
