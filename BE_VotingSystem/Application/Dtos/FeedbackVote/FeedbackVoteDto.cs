namespace BE_VotingSystem.Application.Dtos.FeedbackVote;

/// <summary>
///     Represents website feedback vote information
/// </summary>
public sealed record FeedbackVoteDto(string Email, int Vote, DateTime VotedAt);
