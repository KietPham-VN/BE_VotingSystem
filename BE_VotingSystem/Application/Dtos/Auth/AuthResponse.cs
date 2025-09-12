namespace BE_VotingSystem.Application.Dtos.Auth;

public sealed record AuthResponse
{
    public string AccessToken { get; init; } = null!;
    public DateTime AccessTokenExpiresAt { get; init; }
    public string RefreshToken { get; init; } = null!;
    public DateTime RefreshTokenExpiresAt { get; init; }
}
