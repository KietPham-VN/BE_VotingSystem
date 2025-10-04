using BE_VotingSystem.Application.Dtos.Account;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Domain.Exceptions;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Service implementation for account management operations
/// </summary>
public class AccountService(IAppDbContext dbContext) : IAccountService
{
    /// <inheritdoc />
    public Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Accounts
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AccountDto(
                a.Id,
                a.StudentCode ?? string.Empty,
                a.Email,
                a.Name ?? string.Empty,
                a.Semester.GetValueOrDefault(),
                a.Department ?? string.Empty,
                a.IsAdmin
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await dbContext.Accounts
            .AsNoTracking()
            .Select(a => new AccountDto(
                a.Id,
                a.StudentCode ?? string.Empty,
                a.Email,
                a.Name ?? string.Empty,
                a.Semester.GetValueOrDefault(),
                a.Department ?? string.Empty,
                a.IsAdmin
            ))
            .ToListAsync(cancellationToken);
        return list;
    }

    /// <inheritdoc />
    public async Task<AccountDto> UpdateAccountAsync(Guid id, UpdateAccountRequest request, Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (account is null)
            throw new InvalidOperationException($"Account with ID '{id}' not found");

        // If trying to change admin status, verify current user is admin
        if (request.IsAdmin.HasValue)
        {
            var currentUser = await dbContext.Accounts
                .FirstOrDefaultAsync(a => a.Id == currentUserId, cancellationToken);

            if (currentUser?.IsAdmin != true)
                throw new ForbiddenException("Only admin users can grant or revoke admin privileges");
        }

        if (request.Name is not null)
            account.Name = request.Name.Trim();

        if (request.StudentCode is not null)
            account.StudentCode = request.StudentCode.Trim();

        if (request.Semester.HasValue)
            account.Semester = request.Semester.Value;

        if (request.Department is not null)
            account.Department = request.Department.Trim();
        if (request.IsAdmin.HasValue)
            account.IsAdmin = request.IsAdmin.Value;


        await dbContext.SaveChangesAsync(cancellationToken);

        return new AccountDto(
            account.Id,
            account.StudentCode ?? string.Empty,
            account.Email,
            account.Name ?? string.Empty,
            account.Semester.GetValueOrDefault(),
            account.Department ?? string.Empty,
            account.IsAdmin
        );
    }

    /// <inheritdoc />
    public async Task DeleteAccountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (account is null)
            throw new InvalidOperationException($"Account with ID '{id}' not found");

        if (account.IsAdmin)
            throw new InvalidOperationException("Cannot delete admin accounts");

        dbContext.Accounts.Remove(account);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AccountDto> BanAccountAsync(Guid id, BanAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (account is null)
            throw new InvalidOperationException($"Account with ID '{id}' not found");

        if (account.IsAdmin)
            throw new InvalidOperationException("Cannot ban admin accounts");

        account.IsBanned = request.IsBanned;
        account.BanReason = request.IsBanned ? request.Reason : null;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AccountDto(
            account.Id,
            account.StudentCode ?? string.Empty,
            account.Email,
            account.Name ?? string.Empty,
            account.Semester.GetValueOrDefault(),
            account.Department ?? string.Empty,
            account.IsAdmin
        );
    }
}