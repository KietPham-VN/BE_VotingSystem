using BE_VotingSystem.Application.Dtos.Common;
using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_VotingSystem.Api.Controllers;

/// <summary>
///     Controller for managing lectures/lecturers
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LectureController(ILecturerService service) : ControllerBase
{
    /// <summary>
    ///     Get all lectures
    /// </summary>
    /// <param name="isActive">Optional filter: true for active, false for inactive, null for all</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of lectures</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all lectures",
        Description = "Retrieves a list of lectures with their vote information, ordered by name. You can filter by activity status using the optional 'isActive' query parameter (true/false). Omitting it returns all.")]
    [ProducesResponseType(typeof(ApiResponse<List<LecturerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<LecturerDto>>>> GetLectures(
        [FromQuery, SwaggerParameter(Description = "Filter by activity status: true=active, false=inactive; omit for all")] bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var lectures = await service.GetLecturers(isActive, cancellationToken);
        return Ok(new ApiResponse<List<LecturerDto>>(lectures, "Fetched lectures successfully"));
    }

    /// <summary>
    ///     Create a new lecture
    /// </summary>
    /// <param name="request">Lecturer creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created lecture</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new lecture",
        Description = "Creates a new lecture with the provided information. Requires admin privileges.")]
    [ProducesResponseType(typeof(Lecturer), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Lecturer>>> CreateLecture(
        [FromBody] CreateLecturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var lecture = await service.AddLecturer(request, cancellationToken);
        return CreatedAtAction(nameof(GetLectures), new { id = lecture.Id },
            new ApiResponse<Lecturer>(lecture, "Lecturer created successfully"));
    }

    /// <summary>
    ///     Update an existing lecture
    /// </summary>
    /// <param name="id">Lecturer ID</param>
    /// <param name="request">Lecturer update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated lecture</returns>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Update a lecture",
        Description = "Updates an existing lecture with the provided information. Requires admin privileges.")]
    [ProducesResponseType(typeof(Lecturer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Lecturer>>> UpdateLecture(
        Guid id,
        [FromBody] CreateLecturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var lecture = await service.UpdateLecturer(id, request, cancellationToken);
        return Ok(new ApiResponse<Lecturer>(lecture, "Lecturer updated successfully"));
    }

    /// <summary>
    ///     Delete a lecture
    /// </summary>
    /// <param name="id">Lecturer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Delete a lecture",
        Description =
            "Deletes a lecture by ID. This will also delete all associated votes. Requires admin privileges.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> DeleteLecture(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await service.DeleteLecturer(id, cancellationToken);
        return Ok(new ApiResponse("Lecturer deleted successfully"));
    }

    /// <summary>
    ///     Activate a lecturer to allow receiving votes
    /// </summary>
    /// <param name="id">Lecturer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpPost("{id:guid}/activate")]
    [SwaggerOperation(
        Summary = "Activate a lecturer",
        Description = "Marks a lecturer as active so they can receive votes. Requires admin privileges.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> ActivateLecturer(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await service.ActivateLecturer(id, cancellationToken);
            return Ok(new ApiResponse("Lecturer activated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    /// <summary>
    ///     Deactivate a lecturer to stop receiving votes
    /// </summary>
    /// <param name="id">Lecturer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpPost("{id:guid}/deactivate")]
    [SwaggerOperation(
        Summary = "Deactivate a lecturer",
        Description = "Marks a lecturer as inactive so they cannot receive votes. Requires admin privileges.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> DeactivateLecturer(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await service.DeactivateLecturer(id, cancellationToken);
            return Ok(new ApiResponse("Lecturer deactivated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }
}
