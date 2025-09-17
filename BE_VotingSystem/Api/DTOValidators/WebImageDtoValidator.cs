using BE_VotingSystem.Application.Dtos.WebImage;
using FluentValidation;

namespace BE_VotingSystem.Api.DTOValidators;

/// <summary>
///     Validator for <see cref="BE_VotingSystem.Application.Dtos.WebImage.WebImageDto"/>.
/// </summary>
public class WebImageDtoValidator : AbstractValidator<WebImageDto>
{
    /// <summary>
    ///     Initializes validation rules for <see cref="WebImageDto"/>.
    /// </summary>
    public WebImageDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage("Image name is required.");

        RuleFor(x => x.ImageUrl)
            .NotNull()
            .NotEmpty()
            .WithMessage("Image URL is required.");
    }
}
