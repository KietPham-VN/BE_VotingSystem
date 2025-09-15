using BE_VotingSystem.Application.Dtos.FeedbackVote;
using BE_VotingSystem.Application.Dtos.FeedbackVote.Requests;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Service implementation for website feedback votes
/// </summary>
public sealed class FeedbackVoteService(IAppDbContext db) : IFeedbackVoteService
{
    /// <summary>
    ///     Creates a website feedback vote for the specified account. Each account can only vote once.
    /// </summary>
    /// <param name="accountId">The account identifier from JWT</param>
    /// <param name="request">The creation request containing the vote value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created feedback vote DTO</returns>
    /// <exception cref="InvalidOperationException">Thrown when the account has already voted</exception>
    public async Task<FeedbackVoteDto> CreateAsync(Guid accountId, CreateFeedbackVoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await db.FeedbackVotes.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == accountId, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("Account has already submitted a feedback vote");

        var entity = new FeedbackVote
        {
            AccountId = accountId,
            Vote = request.Vote,
            VotedAt = DateTime.UtcNow
        };
        db.FeedbackVotes.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new FeedbackVoteDto(entity.Vote, entity.VotedAt);
    }

    /// <summary>
    ///     Updates the website feedback vote for the specified account.
    /// </summary>
    /// <param name="accountId">The account identifier from JWT</param>
    /// <param name="request">The update request containing the new vote value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated feedback vote DTO</returns>
    /// <exception cref="InvalidOperationException">Thrown when no existing vote is found</exception>
    public async Task<FeedbackVoteDto> UpdateAsync(Guid accountId, UpdateFeedbackVoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.FeedbackVotes.FirstOrDefaultAsync(x => x.AccountId == accountId, cancellationToken);
        if (entity is null)
            throw new InvalidOperationException("No existing feedback vote to update");

        entity.Vote = request.Vote;
        entity.VotedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return new FeedbackVoteDto(entity.Vote, entity.VotedAt);
    }

    /// <summary>
    ///     Retrieves the current website feedback vote for the specified account, if any.
    /// </summary>
    /// <param name="accountId">The account identifier from JWT</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The feedback vote DTO or null</returns>
    public Task<FeedbackVoteDto?> GetAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return db.FeedbackVotes.AsNoTracking()
            .Where(x => x.AccountId == accountId)
            .Select(x => new FeedbackVoteDto(x.Vote, x.VotedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
