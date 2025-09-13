using BE_VotingSystem.Application.Dtos.Account;

namespace BE_VotingSystem.Application.Interfaces.Services;

public interface IAccountService
{
    Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<AccountDto> UpdateAccountAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken = default);

    Task DeleteAccountAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AccountDto> BanAccountAsync(Guid id, BanAccountRequest request, CancellationToken cancellationToken = default);
}