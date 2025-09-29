namespace BE_VotingSystem.Application.Dtos.Lecture;

/// <summary>
///     Data transfer object for lecturer information
/// </summary>
/// <param name="Id">Unique identifier for the lecturer</param>
/// <param name="Name">Full name of the lecturer</param>
/// <param name="Email">Lecturer's email</param>
/// <param name="Department">Department the lecturer belongs to</param>
/// <param name="Quote">Personal quote or motto of the lecturer</param>
/// <param name="AvatarUrl">URL to the lecturer's avatar image</param>
/// <param name="Votes">Total number of votes received</param>
/// <param name="IsVoted">Whether the current account has voted for this lecturer today</param>
public sealed record LecturerDto(
    Guid Id,
    string Name,
    string Email,
    string Department,
    string Quote,
    string AvatarUrl,
    int Votes,
    bool IsVoted
);
