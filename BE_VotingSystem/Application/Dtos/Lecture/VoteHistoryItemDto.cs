namespace BE_VotingSystem.Application.Dtos.Lecture;

/// <summary>
///     Represents a single lecturer vote entry in a user's history
/// </summary>
public sealed record VoteHistoryItemDto(string LectureName, string DepartmentName, DateOnly VotedAt);
