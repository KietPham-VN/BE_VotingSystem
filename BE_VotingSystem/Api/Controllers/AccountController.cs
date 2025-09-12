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
}