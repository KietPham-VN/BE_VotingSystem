namespace BE_VotingSystem.Infrastructure.Setting;

/// <summary>
/// Configuration settings for JWT token generation
/// </summary>
public sealed class JwtSettings
{
    /// <summary>
    /// Configuration section name for JWT settings
    /// </summary>
    public const string SectionName = "Jwt";
    
    /// <summary>
    /// JWT token issuer
    /// </summary>
    public string Issuer { get; init; } = null!;
    
    /// <summary>
    /// JWT token audience
    /// </summary>
    public string Audience { get; init; } = null!;
    
    /// <summary>
    /// Secret key for JWT token signing
    /// </summary>
    public string Key { get; init; } = null!;
    
    /// <summary>
    /// Access token lifetime in minutes
    /// </summary>
    public int AccessTokenLifetimeMinutes { get; init; }
    
    /// <summary>
    /// Refresh token lifetime in days
    /// </summary>
    public int RefreshTokenLifetimeDays { get; init; }
}