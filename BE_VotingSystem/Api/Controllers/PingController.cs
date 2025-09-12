using BE_VotingSystem.Application.Dtos.PingDTO;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_VotingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController :  ControllerBase
{
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
