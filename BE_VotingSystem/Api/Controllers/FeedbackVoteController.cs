using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BE_VotingSystem.Application.Dtos.FeedbackVote;
using BE_VotingSystem.Application.Dtos.FeedbackVote.Requests;
using BE_VotingSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_VotingSystem.Api.Controllers;

/// <summary>
///     Controller for website feedback votes (1-5 stars). One vote per account.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class FeedbackVoteController(IFeedbackVoteService service) : ControllerBase
{
    private static Guid GetAccountId(ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }

    /// <summary>
    ///     Get all website feedback votes
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all website feedback votes")]
    [ProducesResponseType(typeof(List<FeedbackVoteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FeedbackVoteDto>>> GetAll(CancellationToken cancellationToken = default)
    {
        var dtos = await service.GetAllAsync(cancellationToken);
        return Ok(dtos);
    }

    /// <summary>
    ///     Create a website feedback vote (1-5). One per account.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create my website feedback vote")]
    [ProducesResponseType(typeof(FeedbackVoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FeedbackVoteDto>> Create([FromBody] CreateFeedbackVoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var accountId = GetAccountId(User);
        try
        {
            var dto = await service.CreateAsync(accountId, request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, dto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    ///     Update my website feedback vote (1-5)
    /// </summary>
    [HttpPatch]
    [SwaggerOperation(Summary = "Update my website feedback vote")]
    [ProducesResponseType(typeof(FeedbackVoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FeedbackVoteDto>> Update([FromBody] UpdateFeedbackVoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var accountId = GetAccountId(User);
        try
        {
            var dto = await service.UpdateAsync(accountId, request, cancellationToken);
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
