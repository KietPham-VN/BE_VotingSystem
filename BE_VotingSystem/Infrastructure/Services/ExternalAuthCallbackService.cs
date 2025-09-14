using System.Security.Claims;
using BE_VotingSystem.Application.Dtos.Auth;
using BE_VotingSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
/// Service for handling external authentication callbacks
/// </summary>
public class ExternalAuthCallbackService(IAuthService authService) : IExternalAuthCallbackService
{
    /// <summary>
    /// Handles Google OAuth callback and returns authentication tokens
    /// </summary>
    /// <param name="httpContext">The HTTP context containing authentication information</param>
    /// <param name="redirectUri">The redirect URI to return to after authentication</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A tuple containing authentication response and redirect URL</returns>
    public async Task<(AuthResponse? tokens, string? redirectUrl)> HandleGoogleCallbackAsync(
        HttpContext httpContext,
        string? redirectUri,
        CancellationToken cancellationToken)
    {
        var result = await httpContext.AuthenticateAsync("External");
        if (result?.Principal == null) return (null, null);

        var principal = result.Principal;

        var provider = "Google";
        var providerId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var name = principal.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(providerId)) return (null, null);

        var tokens = await authService.LoginExternalAsync(provider, providerId, email!, name!, cancellationToken);

        if (string.IsNullOrWhiteSpace(redirectUri)) return (tokens, null);

        var uri =
            $"{redirectUri}?access_token={Uri.EscapeDataString(tokens!.AccessToken)}&refresh_token={Uri.EscapeDataString(tokens.RefreshToken)}";
        return (tokens, uri);
    }
}