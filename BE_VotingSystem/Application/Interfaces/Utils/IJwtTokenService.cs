using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Application.Interfaces.Utils;

public interface IJwtTokenService
{
    string CreateAccessToken(Account account);

    (string token, DateTime expiresAt) CreateRefreshToken();
}