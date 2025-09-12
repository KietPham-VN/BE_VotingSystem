namespace BE_VotingSystem.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; init; }
    public string? Token { get; init; }
    public DateTime Expires { get; init; }
    public Guid AccountId { get; init; }

    public Account Account { get; init; } = null!;
}