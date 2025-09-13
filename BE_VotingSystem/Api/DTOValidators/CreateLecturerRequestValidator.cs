using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using FluentValidation;

namespace BE_VotingSystem.Api.DTOValidators;

public class CreateLecturerRequestValidator : AbstractValidator<CreateLecturerRequest>
{
    public CreateLecturerRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Lecture name is required.")
            .MaximumLength(255).WithMessage("Lecture name cannot exceed 255 characters.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Lecture name cannot be empty or whitespace.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(255).WithMessage("Department cannot exceed 255 characters.")
            .Must(department => !string.IsNullOrWhiteSpace(department)).WithMessage("Department cannot be empty or whitespace.");

        RuleFor(x => x.Quote)
            .MaximumLength(1000).WithMessage("Quote cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Quote));

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(500).WithMessage("Avatar URL cannot exceed 500 characters.")
            .Must(BeValidUrl).WithMessage("Avatar URL must be a valid URL format.")
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl));
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
