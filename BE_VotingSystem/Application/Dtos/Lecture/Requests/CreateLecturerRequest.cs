namespace BE_VotingSystem.Application.Dtos.Lecture.Requests;

/// <summary>
///     Request object for creating a new lecturer
/// </summary>
/// <param name="AccountName">Account name of the lecturer for the uni system</param>
/// <param name="Name">Full name of the lecturer</param>
/// <param name="Email">Lecturer's email</param>
/// <param name="Department">Department the lecturer belongs to</param>
/// <param name="Quote">Personal quote or motto of the lecturer</param>
/// <param name="AvatarUrl">URL to the lecturer's avatar image</param>
public record CreateLecturerRequest(string? AccountName, string Name, string Email, string Department, string Quote, string AvatarUrl);
