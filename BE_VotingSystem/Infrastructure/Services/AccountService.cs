using BE_VotingSystem.Application.Dtos.Account;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
/// Service implementation for account management operations
/// </summary>
public class AccountService(IAppDbContext dbContext) : IAccountService
{
    /// <summary>
    /// Gets an account by its unique identifier
    /// </summary>
    /// <param name="id">The account identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The account DTO if found, otherwise null</returns>
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

    /// <summary>
    /// Gets all accounts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A read-only list of account DTOs</returns>
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

    /// <summary>
    /// Updates an account with the provided information
    /// </summary>
    /// <param name="id">The account identifier</param>
    /// <param name="request">The update request containing new account information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated account DTO</returns>
    /// <exception cref="InvalidOperationException">Thrown when account is not found</exception>
    public async Task<AccountDto> UpdateAccountAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (account == null)
            throw new InvalidOperationException($"Account with ID '{id}' not found");

        // Update properties if provided
        if (request.Name != null)
            account.Name = request.Name.Trim();
        
        if (request.StudentCode != null)
            account.StudentCode = request.StudentCode.Trim();
        
        if (request.Semester.HasValue)
            account.Semester = request.Semester.Value;
        
        if (request.Department != null)
            account.Department = request.Department.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AccountDto(
            account.StudentCode ?? string.Empty,
            account.Email,
            account.Name ?? string.Empty,
            account.Semester.GetValueOrDefault(),
            account.Department ?? string.Empty
        );
    }

    /// <summary>
    /// Deletes an account by its unique identifier
    /// </summary>
    /// <param name="id">The account identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="InvalidOperationException">Thrown when account is not found or is an admin account</exception>
    public async Task DeleteAccountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (account == null)
            throw new InvalidOperationException($"Account with ID '{id}' not found");

        // Prevent deleting admin accounts
        if (account.IsAdmin)
            throw new InvalidOperationException("Cannot delete admin accounts");

        dbContext.Accounts.Remove(account);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Bans or unbans an account
    /// </summary>
    /// <param name="id">The account identifier</param>
    /// <param name="request">The ban request containing ban status and reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated account DTO</returns>
    /// <exception cref="InvalidOperationException">Thrown when account is not found or is an admin account</exception>
    public async Task<AccountDto> BanAccountAsync(Guid id, BanAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (account == null)
            throw new InvalidOperationException($"Account with ID '{id}' not found");

        // Prevent banning admin accounts
        if (account.IsAdmin)
            throw new InvalidOperationException("Cannot ban admin accounts");

        // Update ban status and reason
        account.IsBanned = request.IsBanned;
        account.BanReason = request.IsBanned ? request.Reason : null;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AccountDto(
            account.StudentCode ?? string.Empty,
            account.Email,
            account.Name ?? string.Empty,
            account.Semester.GetValueOrDefault(),
            account.Department ?? string.Empty
        );
    }
}