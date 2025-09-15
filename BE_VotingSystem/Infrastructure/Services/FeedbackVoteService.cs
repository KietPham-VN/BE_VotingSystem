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
    /// <inheritdoc />
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

        var email = await db.Accounts.AsNoTracking()
            .Where(a => a.Id == accountId)
            .Select(a => a.Email)
            .FirstAsync(cancellationToken);
        return new FeedbackVoteDto(email, entity.Vote, entity.VotedAt);
    }

    /// <inheritdoc />
    public async Task<FeedbackVoteDto> UpdateAsync(Guid accountId, UpdateFeedbackVoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.FeedbackVotes.FirstOrDefaultAsync(x => x.AccountId == accountId, cancellationToken);
        if (entity is null)
            throw new InvalidOperationException("No existing feedback vote to update");

        entity.Vote = request.Vote;
        entity.VotedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        var email = await db.Accounts.AsNoTracking()
            .Where(a => a.Id == accountId)
            .Select(a => a.Email)
            .FirstAsync(cancellationToken);
        return new FeedbackVoteDto(email, entity.Vote, entity.VotedAt);
    }

    /// <inheritdoc />
    public Task<FeedbackVoteDto?> GetAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return db.FeedbackVotes.AsNoTracking()
            .Where(x => x.AccountId == accountId)
            .Join(db.Accounts.AsNoTracking(), fv => fv.AccountId, a => a.Id,
                (fv, a) => new FeedbackVoteDto(a.Email, fv.Vote, fv.VotedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FeedbackVoteDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await db.FeedbackVotes.AsNoTracking()
            .Join(db.Accounts.AsNoTracking(), fv => fv.AccountId, a => a.Id,
                (fv, a) => new { fv, a.Email })
            .OrderByDescending(x => x.fv.VotedAt)
            .Select(x => new FeedbackVoteDto(x.Email, x.fv.Vote, x.fv.VotedAt))
            .ToListAsync(cancellationToken);
        return list;
    }
}
