using BE_VotingSystem.Application.Dtos.Account;

namespace BE_VotingSystem.Application.Interfaces.Services;

/// <summary>
///     Service interface for account management operations
/// </summary>
public interface IAccountService
{
    /// <summary>
    ///     Gets an account by its ID
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account DTO if found, null otherwise</returns>
    Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all accounts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of account DTOs</returns>
    Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an account with the provided information
    /// </summary>
    /// <param name="id">Account ID to update</param>
    /// <param name="request">Update request containing new account information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated account DTO</returns>
    Task<AccountDto> UpdateAccountAsync(Guid id, UpdateAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an account by its ID
    /// </summary>
    /// <param name="id">Account ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task DeleteAccountAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Bans or unbans an account
    /// </summary>
    /// <param name="id">Account ID to ban/unban</param>
    /// <param name="request">Ban request containing ban status and reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated account DTO</returns>
    Task<AccountDto> BanAccountAsync(Guid id, BanAccountRequest request, CancellationToken cancellationToken = default);
}