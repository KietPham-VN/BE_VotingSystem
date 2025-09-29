namespace BE_VotingSystem.Infrastructure.Setting;

/// <summary>
///     Configuration settings for Google OAuth authentication
/// </summary>
public sealed class GoogleSettings
{
    /// <summary>
    ///     Configuration section name for Google settings
    /// </summary>
    public const string SectionName = "Authentication:Google";

    /// <summary>
    ///     Google OAuth client ID
    /// </summary>
    public string ClientId { get; init; } = null!;

    /// <summary>
    ///     Google OAuth client secret
    /// </summary>
    public string ClientSecret { get; init; } = null!;

    /// <summary>
    ///     OAuth callback path
    /// </summary>
    public string CallbackPath { get; init; } = "/signin-google";
}