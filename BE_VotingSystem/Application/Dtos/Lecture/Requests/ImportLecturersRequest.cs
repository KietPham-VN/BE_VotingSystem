namespace BE_VotingSystem.Application.Dtos.Lecture.Requests;

/// <summary>
/// Request DTO for importing lecturers from Excel file
/// </summary>
public class ImportLecturersRequest
{
    /// <summary>
    /// Excel file containing lecturer data
    /// </summary>
    public required IFormFile File { get; set; }
}
