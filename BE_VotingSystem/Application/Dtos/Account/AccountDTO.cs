namespace BE_VotingSystem.Application.Dtos.Account;

/// <summary>
///     Data transfer object for account information
/// </summary>
/// <param name="Id">id</param>
/// <param name="StudentCode">Student code identifier</param>
/// <param name="Email">Account email address</param>
/// <param name="Name">Full name of the account holder</param>
/// <param name="Semester">Current semester (0-9)</param>
/// <param name="Department">Department name</param>
public sealed record AccountDto(
    Guid Id,
    string StudentCode,
    string Email,
    string Name,
    byte Semester,
    string Department);
