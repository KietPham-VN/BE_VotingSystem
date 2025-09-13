namespace BE_VotingSystem.Domain.Entities;

public class LectureVote
{
    public Guid Id { get; set; }
    public Guid LectureId { get; set; }
    public Lecture? Lecture { get; set; }

    public Guid AccountId { get; set; }
    public Account? Account { get; set; }

    public DateOnly VotedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}
