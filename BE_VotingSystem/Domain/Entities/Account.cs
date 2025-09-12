using BE_VotingSystem.Domain.Enums;

namespace BE_VotingSystem.Domain.Entities;

public class Account
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Name { get; set; }
    public string? StudentCode { get; set; }
    public byte? Semester { get; set; }

    public string? Department { get; set; }
    public bool IsAdmin { get; set; } = false;
    public AuthProvider Provider { get; set; }
    public string? ProviderId { get; set; }

    public string? PasswordHash { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
