using BE_VotingSystem.Application.Interfaces.Services;

namespace BE_VotingSystem.Api.Controllers;

/// <summary>
///     Controller for exporting data to various formats
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExportController : ControllerBase
{
    private readonly IReportService _reportService;

    /// <summary>
    ///     Initializes a new instance of the ExportController class
    /// </summary>
    /// <param name="reportService">Service for generating reports</param>
    public ExportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    ///     Exports a comprehensive voting report as Excel file
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file containing all voting data</returns>
    [HttpGet("voting-report")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetVotingReport(CancellationToken cancellationToken = default)
    {
        try
        {
            var reportBytes = await _reportService.GenerateVotingReportAsync(cancellationToken);
            var fileName = $"VotingReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error generating voting report", error = ex.Message });
        }
    }

    /// <summary>
    ///     Exports a lecturer performance report as Excel file
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file containing lecturer data</returns>
    [HttpGet("lecturer-report")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLecturerReport(CancellationToken cancellationToken = default)
    {
        try
        {
            var reportBytes = await _reportService.GenerateLecturerReportAsync(cancellationToken);
            var fileName = $"LecturerReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error generating lecturer report", error = ex.Message });
        }
    }

    /// <summary>
    ///     Exports a feedback summary report as Excel file
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file containing feedback data</returns>
    [HttpGet("feedback-report")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFeedbackReport(CancellationToken cancellationToken = default)
    {
        try
        {
            var reportBytes = await _reportService.GenerateFeedbackReportAsync(cancellationToken);
            var fileName = $"FeedbackReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error generating feedback report", error = ex.Message });
        }
    }
}