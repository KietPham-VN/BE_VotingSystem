namespace BE_VotingSystem.Application.Dtos.FeedbackVote.Requests;

/// <summary>
///     Request payload to create a feedback vote for the website
/// </summary>
public sealed record CreateFeedbackVoteRequest(int Vote);
