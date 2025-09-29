using BE_VotingSystem.Application.Dtos.Lecture.Requests;

namespace BE_VotingSystem.Api.DTOValidators;

/// <summary>
///     Validator for CreateLectureVoteRequest
/// </summary>
public sealed class CreateLectureVoteRequestValidator : AbstractValidator<CreateLectureVoteRequest>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CreateLectureVoteRequestValidator"/> class.
    /// </summary>
    public CreateLectureVoteRequestValidator()
    {
        RuleFor(x => x.LectureId)
            .NotEmpty().WithMessage("LectureId is required");
    }
}