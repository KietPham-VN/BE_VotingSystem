using BE_VotingSystem.Application.Dtos.Account;
using BE_VotingSystem.Application.Dtos.FeedbackVote;
using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Interfaces.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Service for generating Excel reports from voting system data
/// </summary>
/// 
/// <param name="lecturerService">Service for lecturer operations</param>
/// <param name="lectureVoteService">Service for lecture vote operations</param>
/// <param name="feedbackVoteService">Service for feedback vote operations</param>
/// <param name="accountService">Service for account operations</param>
/// <param name="environment">Use to configure path</param>
public class ReportService(
    ILecturerService lecturerService,
    ILectureVoteService lectureVoteService,
    IFeedbackVoteService feedbackVoteService,
    IAccountService accountService,
    IWebHostEnvironment environment) : IReportService
{
    /// <summary>
    ///     Generates a comprehensive voting report with all data
    /// </summary>
    public async Task<byte[]> GenerateVotingReportAsync(CancellationToken cancellationToken = default)
    {
        var templatePath = Path.Combine(environment.ContentRootPath, "Resources", "Templates", "Report.xlsx");

        if (!File.Exists(templatePath)) throw new FileNotFoundException($"Template file not found at: {templatePath}");

        using var package = new ExcelPackage(new FileInfo(templatePath));

        await InspectTemplateStructure(package);

        var lecturers = await lecturerService.GetLecturers(cancellationToken: cancellationToken);
        var feedbackVotes = await feedbackVoteService.GetAllAsync(cancellationToken);
        var accounts = await accountService.GetAllAsync(cancellationToken);

        await FillDataIntoTemplate(package, lecturers, feedbackVotes, accounts, cancellationToken);

        return await package.GetAsByteArrayAsync(cancellationToken);
    }

    /// <summary>
    ///     Generates a lecturer performance report
    /// </summary>
    public async Task<byte[]> GenerateLecturerReportAsync(CancellationToken cancellationToken = default)
    {
        var templatePath = Path.Combine(environment.ContentRootPath, "Resources", "Templates", "Report.xlsx");

        if (!File.Exists(templatePath)) throw new FileNotFoundException($"Template file not found at: {templatePath}");

        using var package = new ExcelPackage(new FileInfo(templatePath));

        var campaignResultSheet = package.Workbook.Worksheets[0];

        var lecturers = await lecturerService.GetLecturers(cancellationToken: cancellationToken);
        await FillCampaignResultData(campaignResultSheet, lecturers, cancellationToken);

        return await package.GetAsByteArrayAsync(cancellationToken);
    }

    /// <summary>
    ///     Generates a feedback summary report
    /// </summary>
    public async Task<byte[]> GenerateFeedbackReportAsync(CancellationToken cancellationToken = default)
    {
        var templatePath = Path.Combine(environment.ContentRootPath, "Resources", "Templates", "Report.xlsx");

        if (!File.Exists(templatePath)) throw new FileNotFoundException($"Template file not found at: {templatePath}");

        using var package = new ExcelPackage(new FileInfo(templatePath));

        var evaluationStatsSheet = package.Workbook.Worksheets[3];

        var feedbackVotes = await feedbackVoteService.GetAllAsync(cancellationToken);
        FillEvaluationStatsData(evaluationStatsSheet, feedbackVotes);

        return await package.GetAsByteArrayAsync(cancellationToken);
    }

    /// <summary>
    ///     Fills data into Sheet 1: 'CAMPAIGN RESULTS' - Lecturer, Total votes, Notes
    /// </summary>
    private async Task FillCampaignResultData(ExcelWorksheet worksheet, List<LecturerDto> lecturers,
        CancellationToken cancellationToken)
    {
        const int startRow = 2;
        var currentRow = startRow;

        foreach (var lecturer in lecturers)
        {
            var votes = await lectureVoteService.GetVotesByLectureAsync(lecturer.Id, cancellationToken);

            worksheet.Cells[currentRow, 1].Value = lecturer.Name;
            worksheet.Cells[currentRow, 2].Value = votes.Count;
            worksheet.Cells[currentRow, 3].Value = lecturer.Quote;

            currentRow++;
        }

        if (currentRow > startRow)
        {
            var range = worksheet.Cells[startRow, 1, currentRow - 1, 3];
            ApplyTableFormatting(range);
        }
    }

    /// <summary>
    ///     Fills data into Sheet 2: 'DETAILED STATISTICS' - Complex layout with departments and statistics
    /// </summary>
    private static void FillDetailedStatsData(ExcelWorksheet worksheet, List<LecturerDto> lecturers,
        IReadOnlyList<FeedbackVoteDto> feedbackVotes, IReadOnlyList<AccountDto> accounts)
    {

        worksheet.Cells[5, 1].Value = "Total lecturers";
        worksheet.Cells[5, 2].Value = lecturers.Count;

        worksheet.Cells[20, 1].Value = "Total participants";
        worksheet.Cells[20, 2].Value = accounts.Count;

        var semester7to9Count = accounts.Count(a => a.Semester is >= 7 and <= 9);
        var semester1to6Count = accounts.Count(a => a.Semester is >= 1 and <= 6);
        var probationCount = accounts.Count(a => a.Semester == 0);

        worksheet.Cells["J3"].Value = semester7to9Count; 
        worksheet.Cells["J4"].Value = semester1to6Count; 
        worksheet.Cells["J5"].Value = probationCount; 
        worksheet.Cells["J6"].Value = accounts.Count;
        var departments = lecturers.GroupBy(l => l.Department).ToList();
        var departmentRow = 3;

        foreach (var department in departments)
        {
            var voteCount = department.Sum(l => l.Votes);

            worksheet.Cells[$"M{departmentRow}"].Value = department.Key; 
            worksheet.Cells[$"N{departmentRow}"].Value = voteCount; 
            departmentRow++;
        }

        var accountDepartments = accounts.GroupBy(a => a.Department).ToList();
        var accountRow = 8;

        foreach (var accountDepartment in accountDepartments) 
        {
            var accountCount = accountDepartment.Count();

            worksheet.Cells[$"F{accountRow}:I{accountRow}"].Merge = true;
            worksheet.Cells[$"F{accountRow}"].Value = accountDepartment.Key; 
            worksheet.Cells[$"J{accountRow}"].Value = accountCount;
            
            worksheet.Cells[$"F{accountRow}"].Style.Font.Size = 11;
            worksheet.Cells[$"F{accountRow}"].Style.Font.Name = "Calibri";
            worksheet.Cells[$"J{accountRow}"].Style.Font.Size = 11;
            worksheet.Cells[$"J{accountRow}"].Style.Font.Name = "Calibri";
            
            accountRow++;
        }


        worksheet.Cells["M21"].Value = "TOTAL";
        worksheet.Cells["N21"].Value = lecturers.Sum(l => l.Votes); 

        worksheet.Cells["J21"].Value = accounts.Count; 

        var range = worksheet.Cells[2, 1, 22, 14];
        ApplyTableFormatting(range);
    }

    /// <summary>
    ///     Fills data into Sheet 3: 'PARTICIPANTS' - Gmail, Lecturer name 1, 2, 3, ...
    /// </summary>
    private async Task FillParticipantsData(ExcelWorksheet worksheet, List<LecturerDto> lecturers,
        IReadOnlyList<AccountDto> accounts, CancellationToken cancellationToken)
    {
        worksheet.Cells[1, 1].Value = "Gmail";
        for (var i = 0; i < lecturers.Count; i++) worksheet.Cells[1, 2 + i].Value = lecturers[i].Name; 

        const int startRow = 2;
        var currentRow = startRow;

        foreach (var account in accounts)
        {
            worksheet.Cells[currentRow, 1].Value = account.Email; 

            for (var i = 0; i < lecturers.Count; i++)
            {
                var votes = await lectureVoteService.GetVotesByLectureAsync(lecturers[i].Id, cancellationToken);
                var hasVoted = votes.Any(v => v.Email == account.Email);
                worksheet.Cells[currentRow, 2 + i].Value = hasVoted ? "✓" : ""; 
            }

            currentRow++;
        }

        if (currentRow > startRow)
        {
            var range = worksheet.Cells[1, 1, currentRow - 1, 1 + lecturers.Count];
            ApplyTableFormatting(range);
        }
    }

    /// <summary>
    ///     Fills data into Sheet 4: 'EVALUATION STATISTICS' - Gmail, Content(Rating), Created Date
    /// </summary>
    private static void FillEvaluationStatsData(ExcelWorksheet worksheet, IReadOnlyList<FeedbackVoteDto> feedbackVotes)
    {
        const int startRow = 2;
        var currentRow = startRow;

        foreach (var feedback in feedbackVotes)
        {
            worksheet.Cells[currentRow, 1].Value = feedback.Email; 
            worksheet.Cells[currentRow, 2].Value = $"{feedback.Vote} stars"; 
            worksheet.Cells[currentRow, 3].Value = feedback.VotedAt.ToString("dd/MM/yyyy HH:mm"); 

            currentRow++;
        }

        if (currentRow <= startRow) return;
        var range = worksheet.Cells[startRow, 1, currentRow - 1, 3];
        ApplyTableFormatting(range);
    }

    /// <summary>
    ///     Inspects the template structure to understand sheet layout
    /// </summary>
    private static Task InspectTemplateStructure(ExcelPackage package)
    {
        Console.WriteLine("=== TEMPLATE STRUCTURE INSPECTION ===");
        Console.WriteLine($"Total Worksheets: {package.Workbook.Worksheets.Count}");

        for (var i = 0; i < package.Workbook.Worksheets.Count; i++)
        {
            var worksheet = package.Workbook.Worksheets[i];
            Console.WriteLine($"Sheet {i + 1}: '{worksheet.Name}'");
            Console.WriteLine($"  - Used Range: {worksheet.Dimension?.Address ?? "Empty"}");
    
            for (var row = 1; row <= Math.Min(5, worksheet.Dimension?.End.Row ?? 0); row++)
            {
                var rowData = new List<string>();
                for (var col = 1; col <= Math.Min(10, worksheet.Dimension?.End.Column ?? 0); col++)
                {
                    var cellValue = worksheet.Cells[row, col].Value?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(cellValue)) rowData.Add(cellValue);
                }

                if (rowData.Count != 0) Console.WriteLine($"  - Row {row}: {string.Join(" | ", rowData)}");
            }

            Console.WriteLine();
        }

        Console.WriteLine("=== END INSPECTION ===");
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Fills data into the appropriate template sheets based on actual template structure
    /// </summary>
    private async Task FillDataIntoTemplate(ExcelPackage package, List<LecturerDto> lecturers,
        IReadOnlyList<FeedbackVoteDto> feedbackVotes, IReadOnlyList<AccountDto> accounts,
        CancellationToken cancellationToken)
    {
        var campaignResultSheet = package.Workbook.Worksheets[0]; 
        var detailedStatsSheet = package.Workbook.Worksheets[1]; 
        var participantsSheet = package.Workbook.Worksheets[2]; 
        var evaluationStatsSheet = package.Workbook.Worksheets[3]; 

        await FillCampaignResultData(campaignResultSheet, lecturers, cancellationToken);
        FillDetailedStatsData(detailedStatsSheet, lecturers, feedbackVotes, accounts);
        await FillParticipantsData(participantsSheet, lecturers, accounts, cancellationToken);
        FillEvaluationStatsData(evaluationStatsSheet, feedbackVotes);
    }


    /// <summary>
    ///     Applies consistent table formatting to a range
    /// </summary>
    private static void ApplyTableFormatting(ExcelRange range)
    {
        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
        range.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
    }
}
