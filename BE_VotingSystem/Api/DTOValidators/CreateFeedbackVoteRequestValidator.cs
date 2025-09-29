using BE_VotingSystem.Application.Dtos.FeedbackVote.Requests;

namespace BE_VotingSystem.Api.DTOValidators;

/// <summary>
///     Validator for <see cref="CreateFeedbackVoteRequest"/>
/// </summary>
public sealed class CreateFeedbackVoteRequestValidator : AbstractValidator<CreateFeedbackVoteRequest>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CreateFeedbackVoteRequestValidator"/> class.
    ///     Ensures that the vote value is within the allowed range (1-5).
    /// </summary>
    public CreateFeedbackVoteRequestValidator()
    {
        RuleFor(x => x.Vote)
            .InclusiveBetween(1, 5)
            .WithMessage("Vote must be between 1 and 5");
    }
}