using BE_VotingSystem.Application.Dtos.Lecture.Requests;

namespace BE_VotingSystem.Api.DTOValidators;

/// <summary>
/// Validator for ImportLecturersRequest
/// </summary>
public class ImportLecturersRequestValidator : AbstractValidator<ImportLecturersRequest>
{
    private static readonly string[] AllowedExtensions = [".xlsx"]; // Restrict to .xlsx to avoid legacy .xls issues
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public ImportLecturersRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required");

        RuleFor(x => x.File)
            .Must(file => file != null && file.Length > 0)
            .WithMessage("File cannot be empty");

        RuleFor(x => x.File)
            .Must(file => file != null && file.Length <= MaxFileSize)
            .WithMessage($"File size cannot exceed {MaxFileSize / (1024 * 1024)}MB");

        RuleFor(x => x.File)
            .Must(file => file != null && HasValidExtension(file.FileName))
            .WithMessage($"File must be one of the following formats: {string.Join(", ", AllowedExtensions)}");

        RuleFor(x => x.File)
            .Must(file => file != null && IsValidMimeType(file.ContentType))
            .WithMessage("File must be a valid Excel file");
    }

    private static bool HasValidExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    private static bool IsValidMimeType(string contentType)
    {
        var validMimeTypes = new[]
        {
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" // .xlsx
        };

        return validMimeTypes.Contains(contentType.ToLowerInvariant());
    }
}