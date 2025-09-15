namespace BE_VotingSystem.Application.Dtos.Lecture.Requests;

/// <summary>
///     Request payload to cancel a vote for a lecturer on the current day
/// </summary>
public sealed record CancelLectureVoteRequest(Guid LectureId);
