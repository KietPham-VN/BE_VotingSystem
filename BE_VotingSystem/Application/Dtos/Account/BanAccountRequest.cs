namespace BE_VotingSystem.Application.Dtos.Account;

/// <summary>
/// Request object for banning or unbanning an account
/// </summary>
/// <param name="IsBanned">Whether the account should be banned</param>
/// <param name="Reason">Optional reason for the ban action</param>
public record BanAccountRequest(
    bool IsBanned,
    string? Reason
);
