using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Application.Interfaces.Services;

public interface ILectureService
{
    Task<List<Lecture>> GetLectures(CancellationToken cancellationToken = default);
    Task<Lecture> AddLecture(CreateLecturerRequest request, CancellationToken cancellationToken = default);
    Task<Lecture> UpdateLecture(Guid id, CreateLecturerRequest request, CancellationToken cancellationToken = default);
    Task DeleteLecture(Guid id, CancellationToken cancellationToken = default);
}
