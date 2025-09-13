namespace BE_VotingSystem.Application.Dtos.Account;

public sealed record UpdateAccountRequest(
    string? Name,
    string? StudentCode,
    byte? Semester,
    string? Department
);
