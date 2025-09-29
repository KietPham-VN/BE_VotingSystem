using BE_VotingSystem.Application.Dtos.PingDTO;

namespace BE_VotingSystem.Api.Controllers;

/// <summary>
///     Controller for health check and ping operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PingController : ControllerBase
{
    /// <summary>
    ///     Health check endpoint that returns pong with current time
    /// </summary>
    /// <returns>Ping response with current UTC time</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PingResponse), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Ping API",
        Description = "Returns 'pong' along with the current UTC time to verify that the API is running."
    )]
    public IActionResult Get()
    {
        return Ok(new PingResponse("Pong!", DateTime.UtcNow));
    }
}