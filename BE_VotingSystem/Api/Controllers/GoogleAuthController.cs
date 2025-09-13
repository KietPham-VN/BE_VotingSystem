using BE_VotingSystem.Application.Dtos.Auth;
using BE_VotingSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_VotingSystem.Api.Controllers;

[ApiController]
[Route("api/google-auth/external")]
public class GoogleAuthController(IExternalAuthCallbackService externalCallbackService) : ControllerBase
{
    [HttpGet("google")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Start Google OAuth",
        Description = "Redirects the browser to Google OAuth consent screen.")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public IActionResult GoogleLogin([FromQuery] string? redirectUri = null)
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.ActionLink(nameof(GoogleCallback), "GoogleAuth", new { redirectUri })
        };
        return Challenge(props, "Google");
    }

    [HttpGet("google-callback")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Google OAuth callback",
        Description =
            "Handles Google's callback, signs the user in (create if needed) and returns or redirects with tokens.")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GoogleCallback([FromQuery] string? redirectUri, CancellationToken ct)
    {
        var (tokens, url) = await externalCallbackService.HandleGoogleCallbackAsync(HttpContext, redirectUri, ct);
        if (tokens is null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(url)) return Ok(tokens);
        return Redirect(url);
    }
}
