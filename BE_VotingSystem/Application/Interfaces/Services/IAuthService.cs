using BE_VotingSystem.Application.Dtos.Auth;

namespace BE_VotingSystem.Application.Interfaces.Services;

/// <summary>
///     Service interface for authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    ///     Authenticates a user with email and password
    /// </summary>
    /// <param name="req">Login request containing email and password</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Authentication response with tokens if successful, null otherwise</returns>
    Task<AuthResponse?> LoginAsync(LoginRequest req, CancellationToken ct);

    /// <summary>
    ///     Authenticates a user through external provider (e.g., Google)
    /// </summary>
    /// <param name="provider">External authentication provider</param>
    /// <param name="providerId">User ID from the external provider</param>
    /// <param name="email">User email from the external provider</param>
    /// <param name="name">User name from the external provider</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Authentication response with tokens if successful, null otherwise</returns>
    Task<AuthResponse?> LoginExternalAsync(string provider, string providerId, string email, string name,
        CancellationToken ct);

    /// <summary>
    ///     Refreshes an access token using a refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to exchange for new access token</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Authentication response with new tokens if successful, null otherwise</returns>
    Task<AuthResponse?> RefreshAsync(string refreshToken, CancellationToken ct);
}