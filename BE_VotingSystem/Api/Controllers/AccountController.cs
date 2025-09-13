using BE_VotingSystem.Application.Dtos.Account;
using BE_VotingSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_VotingSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List all accounts", Description = "Returns 200 with all accounts")]
    [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var accounts = await accountService.GetAllAsync(ct);
        return Ok(accounts);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get account by id", Description = "Returns 200 with account or 404 if not found")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var account = await accountService.GetByIdAsync(id);
        if (account is null) return NotFound();

        return Ok(account);
    }

    /// <summary>
    /// Update an account
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated account</returns>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Update an account",
        Description = "Updates account information. Requires admin privileges.")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AccountDto>> UpdateAccount(
        Guid id,
        [FromBody] UpdateAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var account = await accountService.UpdateAccountAsync(id, request, cancellationToken);
        return Ok(account);
    }

    /// <summary>
    /// Delete an account
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Delete an account",
        Description = "Deletes an account by ID. Cannot delete admin accounts. Requires admin privileges.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAccount(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await accountService.DeleteAccountAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Ban or unban an account
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="request">Ban request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated account</returns>
    [HttpPatch("{id:guid}/ban")]
    [SwaggerOperation(
        Summary = "Ban or unban an account",
        Description = "Bans or unbans an account. Cannot ban admin accounts. Requires admin privileges.")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AccountDto>> BanAccount(
        Guid id,
        [FromBody] BanAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var account = await accountService.BanAccountAsync(id, request, cancellationToken);
        return Ok(account);
    }
}