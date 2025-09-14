using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Application.Interfaces.Utils;

/// <summary>
///     Service interface for JWT token operations
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    ///     Creates a JWT access token for the specified account
    /// </summary>
    /// <param name="account">Account to create token for</param>
    /// <returns>JWT access token string</returns>
    string CreateAccessToken(Account account);

    /// <summary>
    ///     Creates a refresh token
    /// </summary>
    /// <returns>Tuple containing refresh token string and expiration date</returns>
    (string token, DateTime expiresAt) CreateRefreshToken();
}