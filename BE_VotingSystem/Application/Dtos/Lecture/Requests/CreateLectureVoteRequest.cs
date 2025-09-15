namespace BE_VotingSystem.Application.Dtos.Lecture.Requests;

/// <summary>
///     Request payload to cast a vote for a lecturer
/// </summary>
public sealed record CreateLectureVoteRequest(Guid LectureId);
