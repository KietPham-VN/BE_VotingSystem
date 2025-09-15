using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Service implementation for lecturer management operations
/// </summary>
public class LecturerService(IAppDbContext context) : ILecturerService
{
    /// <summary>
    ///     Gets all lecturers with their vote counts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of lecturer DTOs ordered by name</returns>
    public async Task<List<LecturerDto>> GetLecturers(CancellationToken cancellationToken = default)
    {
        return await context.Lectures
            .AsNoTracking()
            .Include(l => l.Votes)
            .OrderBy(l => l.Name)
            .Select(l => new LecturerDto(
                l.Id,
                l.Name ?? string.Empty,
                l.Department ?? string.Empty,
                l.Quote ?? string.Empty,
                l.AvatarUrl ?? string.Empty,
                l.Votes.Count
            ))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Adds a new lecturer
    /// </summary>
    /// <param name="request">The create lecturer request containing lecturer information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created lecture entity</returns>
    /// <exception cref="InvalidOperationException">Thrown when a lecturer with the same name already exists</exception>
    public async Task<Lecture> AddLecturer(CreateLecturerRequest request, CancellationToken cancellationToken = default)
    {
        var existingLecture = await context.Lectures
            .FirstOrDefaultAsync(
                l => l.Name != null && l.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase),
                cancellationToken);


        if (existingLecture is not null)
            throw new InvalidOperationException($"Lecture with name '{request.Name}' already exists");

        var lecture = new Lecture
        {
            Name = request.Name.Trim(),
            Department = request.Department.Trim(),
            Quote = request.Quote?.Trim(),
            AvatarUrl = request.AvatarUrl?.Trim()
        };

        context.Lectures.Add(lecture);
        await context.SaveChangesAsync(cancellationToken);

        return lecture;
    }

    /// <summary>
    ///     Updates an existing lecturer
    /// </summary>
    /// <param name="id">The lecturer identifier</param>
    /// <param name="request">The create lecturer request containing updated information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated lecture entity</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when lecturer is not found or another lecturer with the same name
    ///     exists
    /// </exception>
    public async Task<Lecture> UpdateLecturer(Guid id, CreateLecturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (lecture is null)
            throw new InvalidOperationException($"Lecture with ID '{id}' not found");

        // Check if another lecture with same name exists (excluding current one)
        var existingLecture = await context.Lectures
            .FirstOrDefaultAsync(
                l => l.Id != id &&
                     l.Name != null &&
                     l.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase),
                cancellationToken);

        if (existingLecture is not null)
            throw new InvalidOperationException($"Lecture with name '{request.Name}' already exists");

        // Update properties
        lecture.Name = request.Name.Trim();
        lecture.Department = request.Department.Trim();
        lecture.Quote = request.Quote?.Trim();
        lecture.AvatarUrl = request.AvatarUrl?.Trim();

        await context.SaveChangesAsync(cancellationToken);
        return lecture;
    }

    /// <summary>
    ///     Deletes a lecturer by its unique identifier
    /// </summary>
    /// <param name="id">The lecturer identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="InvalidOperationException">Thrown when lecturer is not found</exception>
    public async Task DeleteLecturer(Guid id, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (lecture is null)
            throw new InvalidOperationException($"Lecture with ID '{id}' not found");

        context.Lectures.Remove(lecture);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    ///     Activates a lecturer by ID so they can receive votes
    /// </summary>
    /// <param name="id">Lecturer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="InvalidOperationException">Thrown when lecturer is not found</exception>
    public async Task ActivateLecturer(Guid id, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (lecture is null)
            throw new InvalidOperationException($"Lecture with ID '{id}' not found");

        if (!lecture.IsActive)
        {
            lecture.IsActive = true;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    ///     Deactivates a lecturer by ID so they cannot receive votes
    /// </summary>
    /// <param name="id">Lecturer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="InvalidOperationException">Thrown when lecturer is not found</exception>
    public async Task DeactivateLecturer(Guid id, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (lecture is null)
            throw new InvalidOperationException($"Lecture with ID '{id}' not found");

        if (lecture.IsActive)
        {
            lecture.IsActive = false;
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
