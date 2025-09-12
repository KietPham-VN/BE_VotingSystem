namespace BE_VotingSystem.Infrastructure.Setting;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string Key { get; init; } = null!;
    public int AccessTokenLifetimeMinutes { get; init; }
    public int RefreshTokenLifetimeDays { get; init; }
}