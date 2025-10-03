using BE_VotingSystem.Application.Dtos.Lecture.Requests;

namespace BE_VotingSystem.Api.DTOValidators;

/// <summary>
///     Validator for GetVoteHistoryRequest
/// </summary>
public sealed class GetVoteHistoryRequestValidator : AbstractValidator<GetVoteHistoryRequest>
{
    public GetVoteHistoryRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);
    }
}
