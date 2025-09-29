using BE_VotingSystem.Application.Dtos.Auth;
using BE_VotingSystem.Application.Interfaces.Services;

namespace BE_VotingSystem.Api.Controllers;

/// <summary>
///     Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    ///     Refreshes access token using refresh token
    /// </summary>
    /// <param name="req">Refresh token request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>New access and refresh tokens</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Refresh access token",
        Description = "Validates a refresh token, rotates it, and returns new access/refresh tokens.")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequest req, CancellationToken ct)
    {
        var tokens = await authService.RefreshAsync(req.RefreshToken, ct);
        if (tokens is null) return Unauthorized();
        return Ok(tokens);
    }

    /// <summary>
    ///     Authenticates user with email and password
    /// </summary>
    /// <param name="req">Login request with email and password</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Access and refresh tokens on successful authentication</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "User Login",
        Description = "Validates credentials and returns JWT access token and refresh token on success."
    )]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var result = await authService.LoginAsync(req, ct);
        if (result is null) return Unauthorized();
        return Ok(result);
    }
}