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
    /// <inheritdoc />
    public async Task<List<LecturerDto>> GetLecturers(bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var query = context.Lectures
            .AsNoTracking()
            .Include(l => l.Votes)
            .AsQueryable();

        if (isActive is not null)
            query = query.Where(l => l.IsActive == isActive);

        return await query
            .OrderBy(l => l.Name)
            .Select(l => new LecturerDto(
                l.Id,
                l.Name ?? string.Empty,
                l.Email ?? string.Empty,
                l.Department ?? string.Empty,
                l.Quote ?? string.Empty,
                l.AvatarUrl ?? string.Empty,
                l.Votes.Count
            ))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Lecturer> AddLecturer(CreateLecturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingLecture = await context.Lectures
            .FirstOrDefaultAsync(
                l => l.Name != null && EF.Functions.Like(l.Name, request.Name),
                cancellationToken);


        if (existingLecture is not null)
            throw new InvalidOperationException($"Lecturer with name '{request.Name}' already exists");

        var lecture = new Lecturer
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Department = request.Department.Trim(),
            Quote = request.Quote.Trim(),
            AvatarUrl = request.AvatarUrl.Trim()
        };

        context.Lectures.Add(lecture);
        await context.SaveChangesAsync(cancellationToken);

        return lecture;
    }

    /// <inheritdoc />
    public async Task<Lecturer> UpdateLecturer(Guid id, CreateLecturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (lecture is null)
            throw new InvalidOperationException($"Lecturer with ID '{id}' not found");

        // Check if another lecture with same name exists (excluding current one)
        var existingLecture = await context.Lectures
            .FirstOrDefaultAsync(
                l => l.Id != id &&
                     l.Name != null &&
                     EF.Functions.Like(l.Name, request.Name),
                cancellationToken);

        if (existingLecture is not null)
            throw new InvalidOperationException($"Lecturer with name '{request.Name}' already exists");

        // Update properties
        lecture.Name = request.Name.Trim();
        lecture.Email = request.Email.Trim();
        lecture.Department = request.Department.Trim();
        lecture.Quote = request.Quote.Trim();
        lecture.AvatarUrl = request.AvatarUrl.Trim();

        await context.SaveChangesAsync(cancellationToken);
        return lecture;
    }

    /// <inheritdoc />
    public async Task DeleteLecturer(Guid id, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (lecture is null)
            throw new InvalidOperationException($"Lecturer with ID '{id}' not found");

        context.Lectures.Remove(lecture);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task ActivateLecturer(Guid id, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (lecture is null)
            throw new InvalidOperationException($"Lecturer with ID '{id}' not found");

        if (!lecture.IsActive)
        {
            lecture.IsActive = true;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task DeactivateLecturer(Guid id, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (lecture is null)
            throw new InvalidOperationException($"Lecturer with ID '{id}' not found");

        if (lecture.IsActive)
        {
            lecture.IsActive = false;
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
