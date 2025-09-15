namespace BE_VotingSystem.Application.Dtos.Lecture;

/// <summary>
///     Represents a single vote for a lecturer on a given day
/// </summary>
public sealed record VoteDto(string Email, DateOnly VotedAt);
