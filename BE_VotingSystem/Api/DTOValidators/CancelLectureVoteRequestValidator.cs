using BE_VotingSystem.Application.Dtos.Lecture.Requests;

namespace BE_VotingSystem.Api.DTOValidators;

/// <summary>
///     Validator for CancelLectureVoteRequest
/// </summary>
public sealed class CancelLectureVoteRequestValidator : AbstractValidator<CancelLectureVoteRequest>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CancelLectureVoteRequestValidator"/> class.
    /// </summary>
    public CancelLectureVoteRequestValidator()
    {
        RuleFor(x => x.LectureId)
            .NotEmpty().WithMessage("LectureId is required");
    }
}