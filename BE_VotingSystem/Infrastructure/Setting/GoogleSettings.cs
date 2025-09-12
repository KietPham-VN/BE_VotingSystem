namespace BE_VotingSystem.Infrastructure.Setting;

public sealed class GoogleSettings
{
    public const string SectionName = "Authentication:Google";
    public string ClientId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
    public string CallbackPath { get; init; } = "/signin-google";
}