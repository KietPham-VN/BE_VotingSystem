using BE_VotingSystem.Application.Dtos.Auth;

namespace BE_VotingSystem.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest req, CancellationToken ct);

    Task<AuthResponse?> LoginExternalAsync(string provider, string providerId, string email, string name,
        CancellationToken ct);

    Task<AuthResponse?> RefreshAsync(string refreshToken, CancellationToken ct);
}