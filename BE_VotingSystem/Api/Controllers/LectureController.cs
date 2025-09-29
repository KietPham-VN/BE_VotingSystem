using BE_VotingSystem.Api.DTOValidators;
using BE_VotingSystem.Application.Dtos.Common;
using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Domain.Entities;
using BE_VotingSystem.Domain.Enums;
using BE_VotingSystem.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace BE_VotingSystem.Api.Controllers;

/// <summary>
///     Controller for managing lectures/lecturers
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LectureController(ILecturerService service, IValidator<ImportLecturersRequest> validator) : ControllerBase
{
    /// <summary>
    ///     Get all lectures
    /// </summary>
    /// <param name="isActive">Optional filter: true for active, false for inactive, null for all</param>
    /// <param name="sortBy">Sort by field: Name, Votes, Department, Email (default: Name)</param>
    /// <param name="order">Order direction: asc for ascending, desc for descending (default: asc)</param>
    /// <param name="top">Number of records to return (default: all)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of lectures</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all lectures",
        Description = "Retrieves a list of lectures with their vote information. Supports filtering by activity status, sorting by various fields, and limiting results.")]
    [ProducesResponseType(typeof(ApiResponse<List<LecturerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<LecturerDto>>>> GetLectures(
        [FromQuery, SwaggerParameter(Description = "Filter by activity status: true=active, false=inactive; omit for all")] bool? isActive,
        [FromQuery, SwaggerParameter(Description = "Sort by field: Name, Votes, Department, Email (default: Name)")] SortBy sortBy = SortBy.Name,
        [FromQuery, SwaggerParameter(Description = "Order direction: asc for ascending, desc for descending (default: asc)")] OrderBy order = OrderBy.Desc,
        [FromQuery, SwaggerParameter(Description = "Number of records to return (default: all)")] int? top = null,
        CancellationToken cancellationToken = default)
    {
        Guid? accountId = null;
        var sub = User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub) 
                  ?? User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (Guid.TryParse(sub, out var parsed)) accountId = parsed;

        var lectures = await service.GetLecturers(accountId, isActive, sortBy, order, top, cancellationToken);
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

    /// <summary>
    ///     Import lecturers from Excel file
    /// </summary>
    /// <param name="request">Import request containing Excel file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Import result with success/failure details</returns>
    [HttpPost("import")]
    [SwaggerOperation(
        Summary = "Import lecturers from Excel",
        Description = "Imports multiple lecturers from an Excel file (.xlsx or .xls). The Excel file must contain columns: AccountName, Name, Email, Department, Quote, AvatarUrl. Requires admin privileges.")]
    [ProducesResponseType(typeof(ApiResponse<ImportLecturersResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ImportLecturersResponse>>> ImportLecturers(
        [FromForm] ImportLecturersRequest request,
        CancellationToken cancellationToken = default)
    {
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<LectureController>>();
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
        
        logger.LogInformation("Import lecturers request started - File: {FileName}, Size: {FileSize} bytes, ContentType: {ContentType} - CorrelationId: {CorrelationId}",
            request.File?.FileName,
            request.File?.Length,
            request.File?.ContentType,
            correlationId);

        // Validate the request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            logger.LogWarning("Import validation failed - Errors: {Errors} - CorrelationId: {CorrelationId}",
                string.Join(", ", errors),
                correlationId);
            return BadRequest(new ApiResponse<ImportLecturersResponse>(null!, $"Validation failed: {string.Join(", ", errors)}"));
        }

        try
        {
            logger.LogInformation("Starting Excel import process - CorrelationId: {CorrelationId}", correlationId);
            var result = await service.ImportLecturersFromExcel(request.File, cancellationToken);
            logger.LogInformation("Excel import completed - Success: {IsSuccess}, Imported: {ImportedCount}, Failed: {FailedCount} - CorrelationId: {CorrelationId}",
                result.IsSuccess,
                result.ImportedCount,
                result.FailedCount,
                correlationId);
            
            if (result.IsSuccess)
            {
                var message = $"Successfully imported {result.ImportedCount} lecturers";
                if (result.FailedCount > 0)
                    message += $", {result.FailedCount} failed";
                
                return Ok(new ApiResponse<ImportLecturersResponse>(result, message));
            }
            else
            {
                var errorMessage = result.Errors.Any() 
                    ? $"Import failed: {string.Join(", ", result.Errors)}"
                    : "Import failed";
                return BadRequest(new ApiResponse<ImportLecturersResponse>(result, errorMessage));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Import lecturers failed - File: {FileName}, Size: {FileSize} bytes - CorrelationId: {CorrelationId}",
                request.File?.FileName,
                request.File?.Length,
                correlationId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse<ImportLecturersResponse>(null!, $"An error occurred during import: {ex.Message}"));
        }
    }

    /// <summary>
    ///     Download Excel template for importing lecturers
    /// </summary>
    /// <returns>Excel template file</returns>
    [HttpGet("import-template")]
    [SwaggerOperation(
        Summary = "Download lecturer import template",
        Description = "Downloads an Excel template file with the correct format for importing lecturers. The template includes sample data and column headers.")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DownloadImportTemplate()
    {
        try
        {
            var templateBytes = ExcelTemplateService.CreateLecturerImportTemplate();
            var fileName = $"LecturerImportTemplate_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            return File(templateBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ApiResponse($"An error occurred while creating template: {ex.Message}"));
        }
    }
}
