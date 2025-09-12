using BE_VotingSystem.Application.Dtos.Account;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;

namespace BE_VotingSystem.Infrastructure.Services;

public class AccountService(IAppDbContext dbContext) : IAccountService
{
    public Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Accounts
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AccountDto(
                a.StudentCode ?? string.Empty,
                a.Email,
                a.Name ?? string.Empty,
                a.Semester.GetValueOrDefault(),
                a.Department ?? string.Empty
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await dbContext.Accounts
            .AsNoTracking()
            .Select(a => new AccountDto(
                a.StudentCode ?? string.Empty,
                a.Email,
                a.Name ?? string.Empty,
                a.Semester.GetValueOrDefault(),
                a.Department ?? string.Empty
            ))
            .ToListAsync(cancellationToken);
        return list;
    }
}
