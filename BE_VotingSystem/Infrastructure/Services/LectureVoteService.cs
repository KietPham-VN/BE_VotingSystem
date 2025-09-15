using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Service implementation for lecturer voting
/// </summary>
public sealed class LectureVoteService(IAppDbContext db) : ILectureVoteService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<VoteDto>> GetVotesByLectureAsync(Guid lectureId,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var votes = await db.LectureVotes.AsNoTracking()
            .Where(v => v.LectureId == lectureId && v.VotedAt == today)
            .Join(db.Accounts.AsNoTracking(), v => v.AccountId, a => a.Id, (v, a) => new VoteDto(a.Email, v.VotedAt))
            .ToListAsync(cancellationToken);
        return votes;
    }

    /// <inheritdoc />
    public async Task<bool> VoteAsync(Guid accountId, CreateLectureVoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Check lecturer exists and active
        var lectureExists = await db.Lectures.AsNoTracking()
            .AnyAsync(l => l.Id == request.LectureId && l.IsActive, cancellationToken);
        if (!lectureExists) throw new InvalidOperationException("Lecturer not found or inactive");

        // Ensure account has remaining votes today
        var account = await db.Accounts.FirstAsync(a => a.Id == accountId, cancellationToken);
        if (account.IsBanned) throw new InvalidOperationException("Account is banned");
        if (account.VotesRemain <= 0) throw new InvalidOperationException("No votes remaining today");

        // Ensure not already voted for this lecturer today
        var alreadyVotedThisLecture = await db.LectureVotes.AsNoTracking()
            .AnyAsync(v => v.AccountId == accountId && v.LectureId == request.LectureId && v.VotedAt == today,
                cancellationToken);
        if (alreadyVotedThisLecture) throw new InvalidOperationException("Already voted for this lecturer today");

        db.LectureVotes.Add(new LecturerVote
        {
            AccountId = accountId,
            LectureId = request.LectureId,
            VotedAt = today
        });
        account.VotesRemain--;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CancelAsync(Guid accountId, CancelLectureVoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var vote = await db.LectureVotes
            .FirstOrDefaultAsync(
                v => v.AccountId == accountId && v.LectureId == request.LectureId && v.VotedAt == today,
                cancellationToken);
        if (vote is null) throw new InvalidOperationException("No vote to cancel for this lecturer today");

        db.LectureVotes.Remove(vote);

        var account = await db.Accounts.FirstAsync(a => a.Id == accountId, cancellationToken);
        // increment back but cap at 3 to be safe
        if (account.VotesRemain < 3) account.VotesRemain++;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}