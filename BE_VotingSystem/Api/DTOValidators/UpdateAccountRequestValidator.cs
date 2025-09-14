using BE_VotingSystem.Application.Dtos.Account;
using FluentValidation;

namespace BE_VotingSystem.Api.DTOValidators;

/// <summary>
/// Validator for UpdateAccountRequest DTO
/// </summary>
public class UpdateAccountRequestValidator : AbstractValidator<UpdateAccountRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateAccountRequestValidator class
    /// </summary>
    public UpdateAccountRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.")
            .Must(name => string.IsNullOrEmpty(name) || !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name cannot be empty or whitespace.")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.StudentCode)
            .MaximumLength(8).WithMessage("Student code cannot exceed 8 characters.")
            .Matches(@"^(SS|SA|SE|CS|CA|CE|HS|HE|HA|QS|QA|QE|DS|DA|DE)[0-9]{6}$")
            .WithMessage("Student code must follow the format: 2 letters followed by 6 digits (e.g., SS123456).")
            .When(x => !string.IsNullOrEmpty(x.StudentCode));

        RuleFor(x => x.Semester)
            .InclusiveBetween<UpdateAccountRequest, byte>(0, 9).WithMessage("Semester must be between 0 and 9.")
            .When(x => x.Semester.HasValue);

        RuleFor(x => x.Department)
            .MaximumLength(255).WithMessage("Department cannot exceed 255 characters.")
            .Must(department => string.IsNullOrEmpty(department) || !string.IsNullOrWhiteSpace(department))
            .WithMessage("Department cannot be empty or whitespace.")
            .When(x => !string.IsNullOrEmpty(x.Department));
    }
}
