namespace BE_VotingSystem.Infrastructure.Setting;

/// <summary>
///     Configuration settings for database connections
/// </summary>
public sealed class DatabaseSettings
{
    /// <summary>
    ///     Configuration section name for database settings
    /// </summary>
    public const string SectionName = "ConnectionStrings";

    /// <summary>
    ///     Default database connection string
    /// </summary>
    public string DefaultConnection { get; init; } = string.Empty;
}