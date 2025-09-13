using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Application.Interfaces.Services;

public interface ILecturerService
{
    Task<List<LecturerDto>> GetLecturers(CancellationToken cancellationToken = default);
    Task<Lecture> AddLecturer(CreateLecturerRequest request, CancellationToken cancellationToken = default);
    Task<Lecture> UpdateLecturer(Guid id, CreateLecturerRequest request, CancellationToken cancellationToken = default);
    Task DeleteLecturer(Guid id, CancellationToken cancellationToken = default);
}
