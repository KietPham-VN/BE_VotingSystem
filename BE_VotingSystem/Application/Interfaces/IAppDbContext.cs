using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Account> Accounts { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
    DbSet<Lecture> Lectures { get; set; }
    DbSet<LectureVote> LectureVotes { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}