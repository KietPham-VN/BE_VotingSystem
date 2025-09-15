using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using FluentValidation;

namespace BE_VotingSystem.Api.DTOValidators;

/// <summary>
///     Validator for CreateLecturerRequest DTO
/// </summary>
public class CreateLecturerRequestValidator : AbstractValidator<CreateLecturerRequest>
{
    /// <summary>
    ///     Initializes a new instance of the CreateLecturerRequestValidator class
    /// </summary>
    public CreateLecturerRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Lecturer name is required.")
            .MaximumLength(255).WithMessage("Lecturer name cannot exceed 255 characters.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Lecturer name cannot be empty or whitespace.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(255).WithMessage("Department cannot exceed 255 characters.")
            .Must(department => !string.IsNullOrWhiteSpace(department))
            .WithMessage("Department cannot be empty or whitespace.");

        RuleFor(x => x.Quote)
            .MaximumLength(1000).WithMessage("Quote cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Quote));

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(500).WithMessage("Avatar URL cannot exceed 500 characters.")
            .Must(BeValidUrl).WithMessage("Avatar URL must be a valid URL format.")
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl));
    }

    /// <summary>
    ///     Validates if the provided string is a valid URL
    /// </summary>
    /// <param name="url">URL string to validate</param>
    /// <returns>True if valid URL, false otherwise</returns>
    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}