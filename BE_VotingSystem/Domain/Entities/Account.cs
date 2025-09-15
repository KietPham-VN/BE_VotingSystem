using BE_VotingSystem.Domain.Enums;

namespace BE_VotingSystem.Domain.Entities;

/// <summary>
///     Represents a user account in the voting system
/// </summary>
public class Account
{
    /// <summary>
    ///     Unique identifier for the account
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Email address of the account
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    ///     Full name of the account holder
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Student code identifier
    /// </summary>
    public string? StudentCode { get; set; }

    /// <summary>
    ///     Current semester (0-9)
    /// </summary>
    public byte? Semester { get; set; }

    /// <summary>
    ///     Number of votes remaining for the account
    /// </summary>
    public byte VotesRemain { get; set; } = 3;

    /// <summary>
    ///     Department name
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    ///     Indicates if the account has admin privileges
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    ///     Indicates if the account is banned
    /// </summary>
    public bool IsBanned { get; set; }

    /// <summary>
    ///     Reason for the ban if the account is banned
    /// </summary>
    public string? BanReason { get; set; }

    /// <summary>
    ///     Authentication provider used for this account
    /// </summary>
    public AuthProvider Provider { get; set; }

    /// <summary>
    ///     External provider ID for this account
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    ///     Hashed password for local authentication
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    ///     Collection of refresh tokens associated with this account
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    /// <summary>
    ///     Collection of votes cast by this account
    /// </summary>
    public ICollection<LecturerVote> Votes { get; set; } = [];

    /// <summary>
    ///     navigation property
    /// </summary>
    public FeedbackVote? FeedbackVote { get; set; }
}