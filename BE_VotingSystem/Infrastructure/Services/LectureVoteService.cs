using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Application.Dtos.Common;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Department categories for semester-based voting rules
/// </summary>
public static class DepartmentCategories
{
    /// <summary>
    ///     Basic subjects departments - can be voted by semester 0 and 1-6
    /// </summary>
    public static readonly HashSet<string> BasicSubjects = new(StringComparer.OrdinalIgnoreCase)
    {
        "Tiếng Anh dự bị",
        "Âm nhạc Truyền thống", 
        "Kỹ năng mềm",
        "Giáo dục thể chất",
        "Toán"
    };

    /// <summary>
    ///     Specialized subjects departments - can be voted by semester 1-6 and 7-9
    /// </summary>
    public static readonly HashSet<string> SpecializedSubjects = new(StringComparer.OrdinalIgnoreCase)
    {
        "Kỹ thuật phần mềm",
        "An toàn thông tin",
        "Trí tuệ nhân tạo",
        "Hệ thống thông tin",
        "Nền tảng máy tính",
        "Quản trị doanh nghiệp",
        "Phát triển khởi nghiệp",
        "Công nghệ truyền thông",
        "Thiết kế mỹ thuật số",
        "Tiếng Anh",
        "Tiếng Nhật",
        "Chính trị"
    };
}

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
    public async Task<IReadOnlyList<VoteDto>> GetAllVotesByLectureAsync(Guid lectureId,
        CancellationToken cancellationToken = default)
    {
        var votes = await db.LectureVotes.AsNoTracking()
            .Where(v => v.LectureId == lectureId)
            .Join(db.Accounts.AsNoTracking(), v => v.AccountId, a => a.Id, (v, a) => new VoteDto(a.Email, v.VotedAt))
            .ToListAsync(cancellationToken);
        return votes;
    }

    /// <inheritdoc />
    public async Task<bool> VoteAsync(Guid accountId, CreateLectureVoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Get account and lecturer information
        var account = await db.Accounts.FirstAsync(a => a.Id == accountId, cancellationToken);
        if (account.IsBanned) throw new InvalidOperationException("Account is banned");
        if (account.VotesRemain <= 0) throw new InvalidOperationException("No votes remaining today");

        var lecturer = await db.Lectures.AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.LectureId && l.IsActive, cancellationToken);
        if (lecturer is null) throw new InvalidOperationException("Lecturer not found or inactive");

        // Validate semester-based voting rules
        await ValidateSemesterBasedVotingAsync(account, lecturer, today, cancellationToken);

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

    /// <summary>
    ///     Validates semester-based voting rules
    /// </summary>
    /// <param name="account">The account attempting to vote</param>
    /// <param name="lecturer">The lecturer being voted for</param>
    /// <param name="today">Today's date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="InvalidOperationException">Thrown when voting rules are violated</exception>
    private async Task ValidateSemesterBasedVotingAsync(Account account, Lecturer lecturer, DateOnly today,
        CancellationToken cancellationToken)
    {
        // If semester is null, allow voting (for backward compatibility)
        if (account.Semester is null) return;

        var semester = account.Semester.Value;
        var lecturerDepartment = lecturer.Department;

        // Get today's votes for this account
        var todayVotes = await db.LectureVotes.AsNoTracking()
            .Where(v => v.AccountId == account.Id && v.VotedAt == today)
            .Join(db.Lectures.AsNoTracking(), v => v.LectureId, l => l.Id, (v, l) => l.Department)
            .ToListAsync(cancellationToken);

        var basicVotesToday = todayVotes.Count(d => DepartmentCategories.BasicSubjects.Contains(d));
        var specializedVotesToday = todayVotes.Count(d => DepartmentCategories.SpecializedSubjects.Contains(d));

        switch (semester)
        {
            case 0: // Semester 0: Can only vote for basic subjects
                if (!DepartmentCategories.BasicSubjects.Contains(lecturerDepartment))
                    throw new InvalidOperationException("Semester 0 students can only vote for basic subject lecturers");
                break;

            case >= 1 and <= 6: // Semester 1-6: Can vote 1 basic + 2 specialized
                if (DepartmentCategories.BasicSubjects.Contains(lecturerDepartment))
                {
                    if (basicVotesToday >= 1)
                        throw new InvalidOperationException("Semester 1-6 students can only vote for 1 basic subject lecturer per day");
                }
                else if (DepartmentCategories.SpecializedSubjects.Contains(lecturerDepartment))
                {
                    if (specializedVotesToday >= 2)
                        throw new InvalidOperationException("Semester 1-6 students can only vote for 2 specialized subject lecturers per day");
                }
                else
                {
                    throw new InvalidOperationException("Lecturer department is not recognized for voting");
                }
                break;

            case >= 7 and <= 9: // Semester 7-9: Can only vote for specialized subjects
                if (!DepartmentCategories.SpecializedSubjects.Contains(lecturerDepartment))
                    throw new InvalidOperationException("Semester 7-9 students can only vote for specialized subject lecturers");
                break;

            default:
                throw new InvalidOperationException($"Invalid semester value: {semester}");
        }
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

    /// <inheritdoc />
    public async Task<PagedResult<VoteHistoryItemDto>> GetMyVoteHistoryAsync(
        Guid accountId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var votesQuery = db.LectureVotes
            .AsNoTracking()
            .Where(v => v.AccountId == accountId);

        var totalCount = await votesQuery.CountAsync(cancellationToken);

        var items = await votesQuery
            .OrderByDescending(v => v.VotedAt)
            .Select(v => new VoteHistoryItemDto(v.Lecture!.Name, v.Lecture!.Department, v.VotedAt))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<VoteHistoryItemDto>(items, totalCount, page, pageSize);
    }
}