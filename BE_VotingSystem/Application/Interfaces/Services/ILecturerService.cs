using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Domain.Entities;
using BE_VotingSystem.Domain.Enums;

namespace BE_VotingSystem.Application.Interfaces.Services;

/// <summary>
///     Service interface for lecturer management operations
/// </summary>
public interface ILecturerService
{
    /// <summary>
    ///     Gets all lecturers with their vote counts
    /// </summary>
    /// <param name="currentAccountId">Current account ID to check if voted today</param>
    /// <param name="isActive">Optional filter by active state</param>
    /// <param name="sortBy">Sort by field (Name, Votes, Department, Email)</param>
    /// <param name="orderBy">Order direction (Asc, Desc)</param>
    /// <param name="top">Number of records to return (null for all)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of lecturer DTOs</returns>
    Task<List<LecturerDto>> GetLecturers(
        Guid? currentAccountId = null,
        bool? isActive = null,
        SortBy sortBy = SortBy.Name,
        OrderBy orderBy = OrderBy.Asc,
        int? top = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a specific lecturer by ID with their vote count
    /// </summary>
    /// <param name="id">Lecturer ID</param>
    /// <param name="currentAccountId">Current account ID to check if voted today</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Lecturer DTO if found, null otherwise</returns>
    Task<LecturerDto?> GetLecturerById(Guid id, Guid? currentAccountId = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds a new lecturer
    /// </summary>
    /// <param name="request">Create lecturer request containing lecturer information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created lecture entity</returns>
    Task<Lecturer> AddLecturer(CreateLecturerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing lecturer
    /// </summary>
    /// <param name="id">Lecturer ID to update</param>
    /// <param name="request">Update request containing new lecturer information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated lecture entity</returns>
    Task<Lecturer> UpdateLecturer(Guid id, CreateLecturerRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a lecturer by ID
    /// </summary>
    /// <param name="id">Lecturer ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task DeleteLecturer(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Activates a lecturer by ID so they can receive votes
    /// </summary>
    /// <param name="id">Lecturer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ActivateLecturer(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deactivates a lecturer by ID so they cannot receive votes
    /// </summary>
    /// <param name="id">Lecturer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeactivateLecturer(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Imports lecturers from Excel file
    /// </summary>
    /// <param name="file">Excel file containing lecturer data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Import result with success/failure details</returns>
    Task<ImportLecturersResponse> ImportLecturersFromExcel(IFormFile file, CancellationToken cancellationToken = default);
}