namespace BE_VotingSystem.Domain.Entities;

public class FeedbackVote
{
    public Guid AccountId { get; set; }
    public int Vote { get; set; }
    public DateTime VotedAt { get; set; }
}
