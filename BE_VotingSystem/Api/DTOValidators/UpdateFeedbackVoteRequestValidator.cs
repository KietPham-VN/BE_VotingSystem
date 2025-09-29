using BE_VotingSystem.Application.Dtos.FeedbackVote.Requests;

namespace BE_VotingSystem.Api.DTOValidators;

/// <summary>
///     Validator for <see cref="UpdateFeedbackVoteRequest"/>
/// </summary>
public sealed class UpdateFeedbackVoteRequestValidator : AbstractValidator<UpdateFeedbackVoteRequest>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UpdateFeedbackVoteRequestValidator"/> class.
    ///     Ensures that the vote value is within the allowed range (1-5).
    /// </summary>
    public UpdateFeedbackVoteRequestValidator()
    {
        RuleFor(x => x.Vote)
            .InclusiveBetween(1, 5)
            .WithMessage("Vote must be between 1 and 5");
    }
}