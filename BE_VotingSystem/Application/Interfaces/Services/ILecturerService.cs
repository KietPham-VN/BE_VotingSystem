using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Application.Interfaces.Services;

/// <summary>
///     Service interface for lecturer management operations
/// </summary>
public interface ILecturerService
{
    /// <summary>
    ///     Gets all lecturers with their vote counts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of lecturer DTOs</returns>
    Task<List<LecturerDto>> GetLecturers(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds a new lecturer
    /// </summary>
    /// <param name="request">Create lecturer request containing lecturer information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created lecture entity</returns>
    Task<Lecture> AddLecturer(CreateLecturerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing lecturer
    /// </summary>
    /// <param name="id">Lecturer ID to update</param>
    /// <param name="request">Update request containing new lecturer information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated lecture entity</returns>
    Task<Lecture> UpdateLecturer(Guid id, CreateLecturerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a lecturer by ID
    /// </summary>
    /// <param name="id">Lecturer ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task DeleteLecturer(Guid id, CancellationToken cancellationToken = default);
}