namespace BE_VotingSystem.Application.Dtos.Lecture.Requests;

public record CreateLecturerRequest(string Name, string Department, string Quote, string AvatarUrl);
