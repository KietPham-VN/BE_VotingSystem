using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Application.Interfaces;

/// <summary>
///     Application database context interface
/// </summary>
public interface IAppDbContext
{
    /// <summary>
    ///     Gets or sets the accounts DbSet
    /// </summary>
    DbSet<Account> Accounts { get; set; }

    /// <summary>
    ///     Gets or sets the refresh tokens DbSet
    /// </summary>
    DbSet<RefreshToken> RefreshTokens { get; set; }

    /// <summary>
    ///     Gets or sets the lectures DbSet
    /// </summary>
    DbSet<Lecture> Lectures { get; set; }

    /// <summary>
    ///     Gets or sets the lecture votes DbSet
    /// </summary>
    DbSet<LectureVote> LectureVotes { get; set; }

    /// <summary>
    ///     Gets or sets the feedback votes DbSet
    /// </summary>
    DbSet<FeedbackVote> FeedbackVotes { get; set; }

    /// <summary>
    ///     Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}