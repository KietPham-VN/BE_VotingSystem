namespace BE_VotingSystem.Application.Dtos.Auth;

/// <summary>
///     Response object containing authentication tokens and expiration information
/// </summary>
public sealed record AuthResponse
{
    /// <summary>
    ///     JWT access token for API authentication
    /// </summary>
    public string AccessToken { get; init; } = null!;

    /// <summary>
    ///     Expiration date and time of the access token
    /// </summary>
    public DateTime AccessTokenExpiresAt { get; init; }

    /// <summary>
    ///     Refresh token for obtaining new access tokens
    /// </summary>
    public string RefreshToken { get; init; } = null!;

    /// <summary>
    ///     Expiration date and time of the refresh token
    /// </summary>
    public DateTime RefreshTokenExpiresAt { get; init; }
}