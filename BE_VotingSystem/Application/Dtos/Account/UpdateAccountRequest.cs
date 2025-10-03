namespace BE_VotingSystem.Application.Dtos.Account;

/// <summary>
///     Request object for updating account information
/// </summary>
/// <param name="Name">Updated name (optional)</param>
/// <param name="StudentCode">Updated student code (optional)</param>
/// <param name="Semester">Updated semester (optional, 0-9)</param>
/// <param name="Department">Updated department (optional)</param>
public sealed record UpdateAccountRequest(
    string? Name,
    string? StudentCode,
    byte? Semester,
    string? Department,
    bool? IsAdmin
);