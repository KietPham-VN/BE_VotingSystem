namespace BE_VotingSystem.Application.Dtos.Lecture.Requests;

/// <summary>
/// Response DTO for importing lecturers from Excel file
/// </summary>
public class ImportLecturersResponse
{
    /// <summary>
    /// Indicates if the import was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Number of lecturers successfully imported
    /// </summary>
    public int ImportedCount { get; set; }

    /// <summary>
    /// Number of lecturers that failed to import
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// List of successfully imported lecturers
    /// </summary>
    public List<LecturerImportResult> ImportedLecturers { get; set; } = [];

    /// <summary>
    /// List of errors that occurred during import
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Detailed error information for each failed row
    /// </summary>
    public List<RowError> RowErrors { get; set; } = [];
}

/// <summary>
/// Result of importing a single lecturer
/// </summary>
public class LecturerImportResult
{
    /// <summary>
    /// Row number in Excel file
    /// </summary>
    public int RowNumber { get; set; }

    /// <summary>
    /// Lecturer's account name
    /// </summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// Lecturer's name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Lecturer's email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Lecturer's department
    /// </summary>
    public string Department { get; set; } = string.Empty;
}

/// <summary>
/// Error information for a specific row
/// </summary>
public class RowError
{
    /// <summary>
    /// Row number where the error occurred
    /// </summary>
    public int RowNumber { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Field that caused the error
    /// </summary>
    public string? Field { get; set; }
}
