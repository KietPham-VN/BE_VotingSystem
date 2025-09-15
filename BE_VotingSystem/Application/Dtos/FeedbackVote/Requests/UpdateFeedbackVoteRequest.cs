namespace BE_VotingSystem.Application.Dtos.FeedbackVote.Requests;

/// <summary>
///     Request payload to update a feedback vote for the website
/// </summary>
public sealed record UpdateFeedbackVoteRequest(int Vote);
