using BE_VotingSystem.Application.Dtos.Auth;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Application.Interfaces.Utils;
using BE_VotingSystem.Domain.Entities;
using BE_VotingSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Service implementation for authentication operations
/// </summary>
public class AuthService(
    IAppDbContext db,
    IPasswordHasher<Account> hasher,
    IJwtTokenService jwt)
    : IAuthService
{
    /// <summary>
    ///     Authenticates a user with email and password
    /// </summary>
    /// <param name="req">The login request containing email and password</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Authentication response with tokens if successful, otherwise null</returns>
    public async Task<AuthResponse?> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await db.Accounts.FirstOrDefaultAsync(x => x.Email == req.Email, ct);
        if (user is null) return null;

        var verify = hasher.VerifyHashedPassword(user, user.PasswordHash ?? string.Empty, req.Password);
        if (verify is PasswordVerificationResult.Failed) return null;

        var access = jwt.CreateAccessToken(user);
        var (refresh, refreshExp) = jwt.CreateRefreshToken();

        db.RefreshTokens.Add(new RefreshToken
            { Token = refresh, Expires = refreshExp, AccountId = user.Id });
        await db.SaveChangesAsync(ct);

        return new AuthResponse
        {
            AccessToken = access,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
            RefreshToken = refresh,
            RefreshTokenExpiresAt = refreshExp
        };
    }

    /// <summary>
    ///     Authenticates a user with external provider (Google, etc.)
    /// </summary>
    /// <param name="provider">The external provider name</param>
    /// <param name="providerId">The user's ID from the external provider</param>
    /// <param name="email">The user's email address</param>
    /// <param name="name">The user's display name</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Authentication response with tokens if successful, otherwise null</returns>
    public async Task<AuthResponse?> LoginExternalAsync(string provider, string providerId, string email, string name,
        CancellationToken ct)
    {
        var user = await db.Accounts.FirstOrDefaultAsync(x => x.Email == email, ct);
        if (user is null)
        {
            user = new Account
            {
                Email = email,
                Name = name,
                Provider = AuthProvider.Google,
                ProviderId = providerId
            };
            db.Accounts.Add(user);
            await db.SaveChangesAsync(ct);
        }

        var access = jwt.CreateAccessToken(user);
        var (refresh, refreshExp) = jwt.CreateRefreshToken();
        db.RefreshTokens.Add(new RefreshToken
            { Token = refresh, Expires = refreshExp, AccountId = user.Id });
        await db.SaveChangesAsync(ct);
        return new AuthResponse
        {
            AccessToken = access,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
            RefreshToken = refresh,
            RefreshTokenExpiresAt = refreshExp
        };
    }

    /// <summary>
    ///     Refreshes an access token using a valid refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token to use for generating new tokens</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>New authentication response with tokens if successful, otherwise null</returns>
    public async Task<AuthResponse?> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var tokenEntity = await db.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == refreshToken, ct);

        if (tokenEntity is null) return null;
        if (tokenEntity.Expires <= DateTime.UtcNow) return null;

        var user = await db.Accounts.FirstOrDefaultAsync(x => x.Id == tokenEntity.AccountId, ct);
        if (user is null) return null;

        var access = jwt.CreateAccessToken(user);
        var (newRefresh, newRefreshExp) = jwt.CreateRefreshToken();

        // rotate token: delete old, insert new
        db.RefreshTokens.Remove(new RefreshToken { Id = tokenEntity.Id });
        db.RefreshTokens.Add(new RefreshToken
            { Token = newRefresh, Expires = newRefreshExp, AccountId = user.Id });
        await db.SaveChangesAsync(ct);

        return new AuthResponse
        {
            AccessToken = access,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
            RefreshToken = newRefresh,
            RefreshTokenExpiresAt = newRefreshExp
        };
    }
}