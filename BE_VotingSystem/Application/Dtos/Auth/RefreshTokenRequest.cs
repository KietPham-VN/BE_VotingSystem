namespace BE_VotingSystem.Application.Dtos.Auth;

/// <summary>
/// Request object for refreshing access token
/// </summary>
/// <param name="RefreshToken">Refresh token to exchange for new access token</param>
public record RefreshTokenRequest(string RefreshToken);