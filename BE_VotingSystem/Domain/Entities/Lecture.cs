namespace BE_VotingSystem.Domain.Entities;

public class Lecture
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Department { get; set; }
    public string? Quote { get; set; }
    public string? AvatarUrl { get; set; }

    public List<LectureVote> Votes { get; set; } = [];
}
