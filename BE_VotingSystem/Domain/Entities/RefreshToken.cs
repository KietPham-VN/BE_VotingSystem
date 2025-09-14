namespace BE_VotingSystem.Domain.Entities;

/// <summary>
/// Represents a refresh token for JWT authentication
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Unique identifier for the refresh token
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// The refresh token string
    /// </summary>
    public string? Token { get; init; }
    
    /// <summary>
    /// Expiration date and time of the refresh token
    /// </summary>
    public DateTime Expires { get; init; }
    
    /// <summary>
    /// ID of the account this refresh token belongs to
    /// </summary>
    public Guid AccountId { get; init; }

    /// <summary>
    /// Navigation property to the account this refresh token belongs to
    /// </summary>
    public Account Account { get; init; } = null!;
}