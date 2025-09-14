namespace BE_VotingSystem.Application.Dtos.Account;

/// <summary>
/// Request object for getting account by ID
/// </summary>
/// <param name="Id">Account ID to retrieve</param>
public sealed record GetAccountRequest(Guid Id);