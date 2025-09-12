using BE_VotingSystem.Application.Interfaces;

namespace BE_VotingSystem.Infrastructure.Services;

public interface IResetVotesService
{
    Task ResetVotesRemainAsync();
}

public class ResetVotesService(IAppDbContext dbContext) : IResetVotesService
{
    public async Task ResetVotesRemainAsync()
    {
        var accounts = await dbContext.Accounts.ToListAsync();
        foreach (var account in accounts.Where(account => account.VotesRemain != 3)) account.VotesRemain = 3;

        await dbContext.SaveChangesAsync();
    }
}
