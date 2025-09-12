using BE_VotingSystem.Application.Dtos.Account;

namespace BE_VotingSystem.Application.Interfaces.Services;

public interface IAccountService
{
    Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default);
}