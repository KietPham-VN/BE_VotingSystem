using BE_VotingSystem.Application.Dtos.Auth;

namespace BE_VotingSystem.Application.Interfaces.Services;

/// <summary>
/// Service interface for handling external authentication callbacks
/// </summary>
public interface IExternalAuthCallbackService
{
    /// <summary>
    /// Handles Google OAuth callback and processes the authentication result
    /// </summary>
    /// <param name="httpContext">HTTP context containing authentication information</param>
    /// <param name="redirectUri">Optional redirect URI after successful authentication</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple containing authentication response and optional redirect URL</returns>
    Task<(AuthResponse? tokens, string? redirectUrl)> HandleGoogleCallbackAsync(
        HttpContext httpContext,
        string? redirectUri,
        CancellationToken cancellationToken);
}