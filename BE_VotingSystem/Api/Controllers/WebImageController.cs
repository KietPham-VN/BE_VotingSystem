using BE_VotingSystem.Application.Dtos.Common;
using BE_VotingSystem.Application.Dtos.WebImage;
using BE_VotingSystem.Application.Interfaces.Services;

namespace BE_VotingSystem.Api.Controllers;

/// <summary>
///     API endpoints for managing web images.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
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
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WebImageDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<WebImageDto>>>> GetImages(CancellationToken cancellationToken)
    {
        var images = await service.GetAllImages(cancellationToken);
        return Ok(new ApiResponse<IEnumerable<WebImageDto>>(images, "Fetched web images successfully"));
    }

    /// <summary>
    ///     Gets a single web image by name.
    /// </summary>
    /// <param name="name">Image name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Web image details.</returns>
    [HttpGet("{name}")]
    [SwaggerOperation(
        Summary = "Get web image by name",
        Description = "Returns a specific web image by its unique name.",
        OperationId = "WebImages_GetByName",
        Tags = ["WebImages"]
    )]
    [ProducesResponseType(typeof(ApiResponse<WebImageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<WebImageDto>>> GetImageByName(string name, CancellationToken cancellationToken)
    {
        var image = await service.GetImageByName(name, cancellationToken);
        
        if (image is null)
            return NotFound(new ApiResponse<WebImageDto>(null!, "Web image not found"));

        return Ok(new ApiResponse<WebImageDto>(image, "Web image retrieved successfully"));
    }

    /// <summary>
    ///     Creates a new web image entry.
    /// </summary>
    /// <param name="request">Web image creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created web image.</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new web image",
        Description = "Creates a new web image entry with the provided name and URL.",
        OperationId = "WebImages_Create",
        Tags = ["WebImages"]
    )]
    [ProducesResponseType(typeof(ApiResponse<WebImageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<WebImageDto>>> CreateImage(
        [FromBody] WebImageDto request,
        CancellationToken cancellationToken = default)
    {
        var image = await service.CreateImage(request, cancellationToken);
        return CreatedAtAction(
            nameof(GetImageByName), 
            new { name = image.Name }, 
            new ApiResponse<WebImageDto>(image, "Web image created successfully"));
    }

    /// <summary>
    ///     Updates an existing web image entry.
    /// </summary>
    /// <param name="request">Web image update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated web image.</returns>
    [HttpPut]
    [SwaggerOperation(
        Summary = "Update a web image",
        Description = "Updates an existing web image's URL by its name. The name cannot be changed.",
        OperationId = "WebImages_Update",
        Tags = ["WebImages"]
    )]
    [ProducesResponseType(typeof(ApiResponse<WebImageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<WebImageDto>>> UpdateImage(
        [FromBody] WebImageDto request,
        CancellationToken cancellationToken = default)
    {
        var image = await service.UpdateImage(request, cancellationToken);
        return Ok(new ApiResponse<WebImageDto>(image, "Web image updated successfully"));
    }

    /// <summary>
    ///     Deletes a web image entry by name.
    /// </summary>
    /// <param name="name">Image name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Deleted web image.</returns>
    [HttpDelete("{name}")]
    [SwaggerOperation(
        Summary = "Delete a web image",
        Description = "Deletes a web image entry by its unique name.",
        OperationId = "WebImages_Delete",
        Tags = ["WebImages"]
    )]
    [ProducesResponseType(typeof(ApiResponse<WebImageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<WebImageDto>>> DeleteImage(
        string name,
        CancellationToken cancellationToken = default)
    {
        var image = await service.DeleteImage(name, cancellationToken);
        return Ok(new ApiResponse<WebImageDto>(image, "Web image deleted successfully"));
    }
}