using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Infrastructure.Services;

public class LecturerService(IAppDbContext context) : ILecturerService
{
    public async Task<List<LecturerDto>> GetLecturers(CancellationToken cancellationToken = default)
    {
        return await context.Lectures
            .AsNoTracking()
            .Include(l => l.Votes)
            .OrderBy(l => l.Name)
            .Select(l => new LecturerDto(
                l.Name ?? string.Empty,
                l.Department ?? string.Empty,
                l.Quote ?? string.Empty,
                l.AvatarUrl ?? string.Empty,
                l.Votes.Count
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<Lecture> AddLecturer(CreateLecturerRequest request, CancellationToken cancellationToken = default)
    {
        var existingLecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Name!.ToLower() == request.Name.ToLower(), cancellationToken);

        if (existingLecture != null)
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

    public async Task<Lecture> UpdateLecturer(Guid id, CreateLecturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (lecture == null)
            throw new InvalidOperationException($"Lecture with ID '{id}' not found");

        // Check if another lecture with same name exists (excluding current one)
        var existingLecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Name!.ToLower() == request.Name.ToLower() && l.Id != id, cancellationToken);

        if (existingLecture != null)
            throw new InvalidOperationException($"Lecture with name '{request.Name}' already exists");

        // Update properties
        lecture.Name = request.Name.Trim();
        lecture.Department = request.Department.Trim();
        lecture.Quote = request.Quote?.Trim();
        lecture.AvatarUrl = request.AvatarUrl?.Trim();

        await context.SaveChangesAsync(cancellationToken);
        return lecture;
    }

    public async Task DeleteLecturer(Guid id, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (lecture == null)
            throw new InvalidOperationException($"Lecture with ID '{id}' not found");

        context.Lectures.Remove(lecture);
        await context.SaveChangesAsync(cancellationToken);
    }
}
