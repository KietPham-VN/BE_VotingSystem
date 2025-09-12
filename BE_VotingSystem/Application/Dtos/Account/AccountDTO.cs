namespace BE_VotingSystem.Application.Dtos.Account;

public sealed record AccountDto(string StudentCode, string Email, string Name, byte Semester, string Department);