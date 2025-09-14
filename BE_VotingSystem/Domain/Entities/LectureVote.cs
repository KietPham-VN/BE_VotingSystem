namespace BE_VotingSystem.Domain.Entities;

/// <summary>
/// Represents a vote cast by an account for a lecturer
/// </summary>
public class LectureVote
{
    /// <summary>
    /// Unique identifier for the vote
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// ID of the lecturer being voted for
    /// </summary>
    public Guid LectureId { get; set; }
    
    /// <summary>
    /// Navigation property to the lecturer being voted for
    /// </summary>
    public Lecture? Lecture { get; set; }

    /// <summary>
    /// ID of the account that cast the vote
    /// </summary>
    public Guid AccountId { get; set; }
    
    /// <summary>
    /// Navigation property to the account that cast the vote
    /// </summary>
    public Account? Account { get; set; }

    /// <summary>
    /// Date when the vote was cast
    /// </summary>
    public DateOnly VotedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}
