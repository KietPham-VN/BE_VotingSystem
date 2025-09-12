namespace BE_VotingSystem.Infrastructure.Setting;

public sealed class DatabaseSettings
{
    public const string SectionName = "ConnectionStrings";
    public string DefaultConnection { get; init; } = string.Empty;
}
