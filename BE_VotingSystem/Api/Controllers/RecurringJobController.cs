using Microsoft.AspNetCore.Mvc;
using Hangfire;
using BE_VotingSystem.Infrastructure.Extensions;

namespace BE_VotingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecurringJobController : ControllerBase
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly ILogger<RecurringJobController> _logger;

    public RecurringJobController(IRecurringJobManager recurringJobManager, ILogger<RecurringJobController> logger)
    {
        _recurringJobManager = recurringJobManager;
        _logger = logger;
    }

    [HttpPost("setup-reset-votes")]
    public IActionResult SetupResetVotesJob()
    {
        try
        {
            _recurringJobManager.RegisterResetVotesJob();
            return Ok(new { Message = "Reset votes remain job setup successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup reset votes job");
            return BadRequest(new { Message = "Failed to setup reset votes job", Error = ex.Message });
        }
    }

    [HttpDelete("remove-reset-votes")]
    public IActionResult RemoveResetVotesJob()
    {
        try
        {
            _recurringJobManager.RemoveResetVotesJob();
            return Ok(new { Message = "Reset votes remain job removed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove reset votes job");
            return BadRequest(new { Message = "Failed to remove reset votes job", Error = ex.Message });
        }
    }
}
