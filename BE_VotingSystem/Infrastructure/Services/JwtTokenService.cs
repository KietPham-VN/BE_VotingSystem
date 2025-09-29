using BE_VotingSystem.Application.Interfaces.Utils;
using BE_VotingSystem.Domain.Entities;
using BE_VotingSystem.Infrastructure.Setting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Service implementation for JWT token operations
/// </summary>
public class JwtTokenService(IOptions<JwtSettings> options) : IJwtTokenService
{
    private readonly JwtSettings _settings = options.Value;

    /// <inheritdoc />
    public string CreateAccessToken(Account account)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, account.Email),
            new Claim("name", account.Name ?? string.Empty),
            new Claim("isAdmin", account.IsAdmin.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _settings.Issuer,
            _settings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenLifetimeMinutes),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc />
    public (string token, DateTime expiresAt) CreateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expires = DateTime.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays);
        return (token, expires);
    }
}