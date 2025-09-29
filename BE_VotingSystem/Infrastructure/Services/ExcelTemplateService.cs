namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Service for creating Excel templates for data import operations
/// </summary>
public static class ExcelTemplateService
{
    static ExcelTemplateService()
    {
    }

    /// <summary>
    ///     Creates a lecturer import template Excel file with predefined headers and sample data
    /// </summary>
    /// <returns>Excel file as byte array containing the template</returns>
    /// <exception cref="InvalidOperationException">Thrown when Excel package creation fails</exception>
    public static byte[] CreateLecturerImportTemplate()
    {
        try
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Lecturers");

            var headers = new[] { "Account", "Họ tên", "Email", "Bộ môn", "Câu nói truyền cảm hứng", "Hình ảnh" };
            for (var i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            var sampleData = new[] { "GV001", "Nguyễn Văn A", "nguyenvana@example.com", "Công nghệ thông tin", "Học tập là chìa khóa thành công", "https://example.com/image.jpg" };
            for (var i = 0; i < sampleData.Length; i++)
            {
                worksheet.Cells[2, i + 1].Value = sampleData[i];
            }

            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create lecturer import template: {ex.Message}", ex);
        }
    }
}