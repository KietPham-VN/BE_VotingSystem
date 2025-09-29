using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Infrastructure.Database.Configurations;

/// <summary>
///     Configuration class for the FeedbackVote entity using Fluent API.
///     Defines the database schema, constraints, and relationships for feedback voting.
/// </summary>
public class FeedbackVoteConfiguration : IEntityTypeConfiguration<FeedbackVote>
{
    /// <summary>
    ///     Configures the FeedbackVote entity using Fluent API.
    /// </summary>
    /// <param name="builder">The entity type builder for FeedbackVote.</param>
    public void Configure(EntityTypeBuilder<FeedbackVote> builder)
    {
        // Configure table name with check constraint using modern syntax
        builder.ToTable("feedback_vote", t => t.HasCheckConstraint("CK_FeedbackVote_Vote", "Vote >= 1 AND Vote <= 5"));

        // Configure primary key using AccountId
        // Each account can only have one feedback vote
        builder.HasKey(fv => fv.AccountId);

        // Configure AccountId property
        builder.Property(fv => fv.AccountId)
            .IsRequired()
            .HasComment("Unique identifier of the account that cast the vote");

        // Configure Vote property
        builder.Property(fv => fv.Vote)
            .IsRequired()
            .HasComment("Vote value between 1 and 5 (star rating)");

        // Configure VotedAt property using modern syntax
        builder.Property(fv => fv.VotedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .HasComment("Date and time when the vote was cast");

        // Configure one-to-one relationship with Account
        builder.HasOne(fv => fv.Account)
            .WithOne(a => a.FeedbackVote)
            .HasForeignKey<FeedbackVote>(fv => fv.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes using modern syntax
        builder.HasIndex(fv => fv.AccountId)
            .IsUnique()
            .HasDatabaseName("IX_feedback_vote_AccountId");

        builder.HasIndex(fv => fv.VotedAt)
            .HasDatabaseName("IX_feedback_vote_VotedAt");
    }
}