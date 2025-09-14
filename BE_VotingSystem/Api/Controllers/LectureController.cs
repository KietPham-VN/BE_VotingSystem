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
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of lectures</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all lectures",
        Description = "Retrieves a list of all lectures with their vote information, ordered by name.")]
    [ProducesResponseType(typeof(List<Lecture>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<LecturerDto
    >>> GetLectures(CancellationToken cancellationToken = default)
    {
        var lectures = await service.GetLecturers(cancellationToken);
        return Ok(lectures);
    }

    /// <summary>
    ///     Create a new lecture
    /// </summary>
    /// <param name="request">Lecture creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created lecture</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new lecture",
        Description = "Creates a new lecture with the provided information. Requires admin privileges.")]
    [ProducesResponseType(typeof(Lecture), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Lecture>> CreateLecture(
        [FromBody] CreateLecturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var lecture = await service.AddLecturer(request, cancellationToken);
        return CreatedAtAction(nameof(GetLectures), new { id = lecture.Id }, lecture);
    }

    /// <summary>
    ///     Update an existing lecture
    /// </summary>
    /// <param name="id">Lecture ID</param>
    /// <param name="request">Lecture update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated lecture</returns>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Update a lecture",
        Description = "Updates an existing lecture with the provided information. Requires admin privileges.")]
    [ProducesResponseType(typeof(Lecture), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Lecture>> UpdateLecture(
        Guid id,
        [FromBody] CreateLecturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var lecture = await service.UpdateLecturer(id, request, cancellationToken);
        return Ok(lecture);
    }

    /// <summary>
    ///     Delete a lecture
    /// </summary>
    /// <param name="id">Lecture ID</param>
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
    public async Task<ActionResult> DeleteLecture(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await service.DeleteLecturer(id, cancellationToken);
        return NoContent();
    }
}