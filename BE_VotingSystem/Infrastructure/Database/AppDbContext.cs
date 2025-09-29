using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Infrastructure.Database;

/// <summary>
///     Entity Framework database context for the voting system
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    /// <summary>
    ///     Gets or sets WebImages
    /// </summary>
    public DbSet<WebImage> WebImages { get; set; }

    /// <summary>
    ///     Gets or sets the accounts DbSet
    /// </summary>
    public DbSet<Account> Accounts { get; set; }

    /// <summary>
    ///     Gets or sets the refresh tokens DbSet
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    /// <summary>
    ///     Gets or sets the lectures DbSet
    /// </summary>
    public DbSet<Lecturer> Lectures { get; set; }

    /// <summary>
    ///     Gets or sets the lecture votes DbSet
    /// </summary>
    public DbSet<LecturerVote> LectureVotes { get; set; }

    /// <summary>
    ///     Gets or sets the feedback votes DbSet
    /// </summary>
    public DbSet<FeedbackVote> FeedbackVotes { get; set; }

    /// <summary>
    ///     Configures the model for the database context
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}