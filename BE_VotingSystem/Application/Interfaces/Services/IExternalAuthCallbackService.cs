using BE_VotingSystem.Application.Dtos.Auth;
using Microsoft.AspNetCore.Http;

namespace BE_VotingSystem.Application.Interfaces.Services;

public interface IExternalAuthCallbackService
{
    Task<(AuthResponse? tokens, string? redirectUrl)> HandleGoogleCallbackAsync(
        HttpContext httpContext,
        string? redirectUri,
        CancellationToken cancellationToken);
}
