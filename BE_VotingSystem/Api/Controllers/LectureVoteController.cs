using BE_VotingSystem.Application.Dtos.Common;
using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Application.Dtos.Common;

namespace BE_VotingSystem.Api.Controllers;

/// <summary>
///     Controller for lecturer voting
/// </summary>
[Authorize]
[ApiController]
[Route("api/lectures/{lectureId:guid}/votes")]
public sealed class LectureVoteController(ILectureVoteService service) : ControllerBase
{
    private static Guid GetAccountId(ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }

    /// <summary>
    ///     Get today's votes by lecture (list of voters' emails)
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get today's votes by lecture")]
    [ProducesResponseType(typeof(List<VoteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<VoteDto>>>> GetByLecture(Guid lectureId, CancellationToken cancellationToken = default)
    {
        var result = await service.GetVotesByLectureAsync(lectureId, cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<VoteDto>>(result, "Fetched votes successfully"));
    }

    /// <summary>
    ///     Get current user's lecturer vote history
    /// </summary>
    [HttpGet("/api/lecture-votes/history")]
    [SwaggerOperation(Summary = "Get my vote history", Description = "Returns paged list of my lecturer votes.")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<VoteHistoryItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedResult<VoteHistoryItemDto>>>> GetMyHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(sub, out var accountId) || accountId == Guid.Empty)
            return Unauthorized(new ApiResponse("Unauthorized"));

        var result = await service.GetMyVoteHistoryAsync(accountId, page, pageSize, cancellationToken);
        return Ok(new ApiResponse<PagedResult<VoteHistoryItemDto>>(result, "Fetched vote history successfully"));
    }

    /// <summary>
    ///     Cast a vote for the lecture. Max 3/day per account, max 1/day per lecture
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Vote for a lecture")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse>> Vote(Guid lectureId, CancellationToken cancellationToken = default)
    {
        var accountId = GetAccountId(User);
        try
        {
            await service.VoteAsync(accountId, new CreateLectureVoteRequest(lectureId), cancellationToken);
            return Ok(new ApiResponse("Voted successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ApiResponse(ex.Message));
        }
    }

    /// <summary>
    ///     Cancel my vote for this lecture today
    /// </summary>
    [HttpDelete]
    [SwaggerOperation(Summary = "Cancel today's vote for a lecture")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Cancel(Guid lectureId, CancellationToken cancellationToken = default)
    {
        var accountId = GetAccountId(User);
        try
        {
            await service.CancelAsync(accountId, new CancelLectureVoteRequest(lectureId), cancellationToken);
            return Ok(new ApiResponse("Vote cancelled successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }
}