using System.Security.Claims;
using BE_VotingSystem.Application.Dtos.Auth;
using BE_VotingSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;

namespace BE_VotingSystem.Infrastructure.Services;

public class ExternalAuthCallbackService(IAuthService authService) : IExternalAuthCallbackService
{
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