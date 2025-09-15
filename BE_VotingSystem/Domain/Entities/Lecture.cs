namespace BE_VotingSystem.Domain.Entities;

/// <summary>
///     Represents a lecturer/lecture in the voting system
/// </summary>
public class Lecture
{
    /// <summary>
    ///     Unique identifier for the lecture
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Name of the lecturer
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Department the lecturer belongs to
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    ///     Personal quote or motto of the lecturer
    /// </summary>
    public string? Quote { get; set; }

    /// <summary>
    ///     URL to the lecturer's avatar image
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    ///     Account status
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    ///     Collection of votes cast for this lecturer
    /// </summary>
    public ICollection<LectureVote> Votes { get; set; } = [];
}
