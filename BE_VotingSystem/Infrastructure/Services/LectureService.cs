using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Infrastructure.Services;

public class LectureService(IAppDbContext context) : ILectureService
{
    public async Task<List<Lecture>> GetLectures(CancellationToken cancellationToken = default)
    {
        return await context.Lectures
            .AsNoTracking()
            .Include(l => l.Votes)
            .OrderBy(l => l.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Lecture> AddLecture(CreateLecturerRequest request, CancellationToken cancellationToken = default)
    {
        var existingLecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (existingLecture != null)
            throw new InvalidOperationException($"Lecture with name '{request.Name}' already exists");

        var lecture = new Lecture
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Department = request.Department.Trim(),
            Quote = request.Quote?.Trim(),
            AvatarUrl = request.AvatarUrl?.Trim()
        };

        context.Lectures.Add(lecture);
        await context.SaveChangesAsync(cancellationToken);

        return lecture;
    }

    public async Task<Lecture> UpdateLecture(Guid id, CreateLecturerRequest request, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (lecture == null)
            throw new InvalidOperationException($"Lecture with ID '{id}' not found");

        // Check if another lecture with same name exists (excluding current one)
        var existingLecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Name.ToLower() == request.Name.ToLower() && l.Id != id, cancellationToken);

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

    public async Task DeleteLecture(Guid id, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (lecture == null)
            throw new InvalidOperationException($"Lecture with ID '{id}' not found");

        context.Lectures.Remove(lecture);
        await context.SaveChangesAsync(cancellationToken);
    }
}
