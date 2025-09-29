namespace BE_VotingSystem.Domain.Entities;

/// <summary>
///     Represents a feedback vote cast by a user for a specific lecture or feedback item.
///     This entity tracks the voting behavior and timing of user feedback submissions.
///     Each account can only have one feedback vote (one-to-one relationship).
/// </summary>
public class FeedbackVote
{
    /// <summary>
    ///     Gets or sets the unique identifier of the account that cast the vote.
    ///     This serves as a foreign key to the Account entity.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    ///     use for navigation
    /// </summary>
    public Account? Account { get; set; }

    /// <summary>
    ///     Gets or sets the vote value cast by the user.
    ///     Must be between 1 and 5 (1-5 star rating system).
    /// </summary>
    public int Vote { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the vote was cast.
    ///     Used for tracking voting history and preventing duplicate votes within time windows.
    /// </summary>
    public DateTime VotedAt { get; set; }
}
