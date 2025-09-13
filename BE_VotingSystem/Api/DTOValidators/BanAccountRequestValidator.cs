using BE_VotingSystem.Application.Dtos.Account;
using FluentValidation;

namespace BE_VotingSystem.Api.DTOValidators;

public class BanAccountRequestValidator : AbstractValidator<BanAccountRequest>
{
    public BanAccountRequestValidator()
    {
        RuleFor(x => x.IsBanned)
            .NotNull().WithMessage("IsBanned field is required.");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Ban reason cannot exceed 500 characters.")
            .Must(reason => string.IsNullOrEmpty(reason) || !string.IsNullOrWhiteSpace(reason))
            .WithMessage("Ban reason cannot be empty or whitespace.")
            .When(x => !string.IsNullOrEmpty(x.Reason));

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ban reason is required when banning an account.")
            .When(x => x.IsBanned);
    }
}
