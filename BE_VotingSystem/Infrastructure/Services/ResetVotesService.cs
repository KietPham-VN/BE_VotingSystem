using BE_VotingSystem.Application.Interfaces;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Interface for resetting votes remain functionality
/// </summary>
public interface IResetVotesService
{
    /// <summary>
    ///     Resets votes remain for all accounts to the default value
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ResetVotesRemainAsync();
}

/// <summary>
///     Service implementation for resetting votes remain functionality
/// </summary>
public class ResetVotesService(IAppDbContext dbContext) : IResetVotesService
{
    /// <inheritdoc/>
    public async Task ResetVotesRemainAsync()
    {
        var accounts = await dbContext.Accounts.ToListAsync();
        foreach (var account in accounts.Where(account => account.VotesRemain != 3)) account.VotesRemain = 3;

        await dbContext.SaveChangesAsync();
    }
}