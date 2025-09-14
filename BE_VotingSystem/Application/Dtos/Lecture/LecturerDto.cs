namespace BE_VotingSystem.Application.Dtos.Lecture;

public sealed record LecturerDto(Guid Id, string Name, string Department, string Quote, string AvatarUrl, int Votes);
