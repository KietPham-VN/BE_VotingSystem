namespace BE_VotingSystem.Application.Dtos.FeedbackVote;

/// <summary>
///     Represents feedback vote information of the current account
/// </summary>
public sealed record FeedbackVoteDto(int Vote, DateTime VotedAt);
