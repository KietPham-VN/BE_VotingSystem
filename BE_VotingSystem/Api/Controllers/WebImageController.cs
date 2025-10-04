using BE_VotingSystem.Application.Dtos.WebImage;
using BE_VotingSystem.Application.Interfaces.Services;

namespace BE_VotingSystem.Api.Controllers;

/// <summary>
///     API endpoints for managing web images.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WebImageController(IWebImageService service) : ControllerBase
{
    /// <summary>
    ///     Gets all web images.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of web images.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all web images",
        Description = "Returns the complete list of web images.",
        OperationId = "WebImages_GetAll",
        Tags = ["WebImages"]
    )]
    [ProducesResponseType(typeof(IEnumerable<WebImageDto>), StatusCodes.Status200OK)]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IEnumerable<WebImageDto>> GetImages(CancellationToken cancellationToken)
    {
        return await service.GetAllImages(cancellationToken);
    }
}