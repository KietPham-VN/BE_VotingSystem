namespace BE_VotingSystem.Application.Dtos.Account;

public record BanAccountRequest(
    bool IsBanned,
    string? Reason
);
