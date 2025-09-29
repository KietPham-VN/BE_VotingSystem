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
/// <param name="lecturerService">Service for lecturer operations</param>
/// <param name="lectureVoteService">Service for lecture vote operations</param>
/// <param name="feedbackVoteService">Service for feedback vote operations</param>
/// <param name="accountService">Service for account operations</param>
/// <param name="environment">Use to configure path</param>
public class ReportService
(
    ILecturerService lecturerService,
    ILectureVoteService lectureVoteService,
    IFeedbackVoteService feedbackVoteService,
    IAccountService accountService,
    IWebHostEnvironment environment
) : IReportService
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
            worksheet.Cells[currentRow, 1].Value = lecturer.Name;
            worksheet.Cells[currentRow, 2].Value = lecturer.Votes;
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
    private async Task FillDetailedStatsData(ExcelWorksheet worksheet, List<LecturerDto> lecturers,
        IReadOnlyList<FeedbackVoteDto> feedbackVotes, IReadOnlyList<AccountDto> accounts,
        CancellationToken cancellationToken)
    {
        // Semester-based participant breakdown (Column A labels, Column B counts)
        worksheet.Cells[2, 1].Value = "Giai đoạn chuyên ngành (HK7-HK9)";
        worksheet.Cells[3, 1].Value = "Giai đoạn chuyên ngành (HK1-HK6)";
        worksheet.Cells[4, 1].Value = "Giai đoạn dự bị";
        worksheet.Cells[5, 1].Value = "Tổng";

        worksheet.Cells[5, 2].Value = accounts.Count;

        var nonAdminAccounts = accounts.Where(a => !a.IsAdmin).ToList();
        var semester7to9Count = nonAdminAccounts.Count(a => a.Semester is >= 7 and <= 9);
        var semester1to6Count = nonAdminAccounts.Count(a => a.Semester is >= 1 and <= 6);
        var probationCount = nonAdminAccounts.Count(a => a.Semester == 0);

        worksheet.Cells["J3"].Value = semester7to9Count;
        worksheet.Cells["J4"].Value = semester1to6Count;
        worksheet.Cells["J5"].Value = probationCount;
        worksheet.Cells["J6"].Value = nonAdminAccounts.Count;

        // Prepare semester groups by email for vote aggregation (Column B)
        var sem7to9Emails = nonAdminAccounts.Where(a => a.Semester is >= 7 and <= 9).Select(a => a.Email)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var sem1to6Emails = nonAdminAccounts.Where(a => a.Semester is >= 1 and <= 6).Select(a => a.Email)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var probationEmails = nonAdminAccounts.Where(a => a.Semester == 0).Select(a => a.Email)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var votesSem7to9 = 0;
        var votesSem1to6 = 0;
        var votesProbation = 0;

        foreach (var lecturer in lecturers)
        {
            var votes = await lectureVoteService.GetAllVotesByLectureAsync(lecturer.Id, cancellationToken);
            if (votes.Count == 0) continue;

            votesSem7to9 += votes.Count(v => sem7to9Emails.Contains(v.Email));
            votesSem1to6 += votes.Count(v => sem1to6Emails.Contains(v.Email));
            votesProbation += votes.Count(v => probationEmails.Contains(v.Email));
        }

        var totalVotesAllGroups = votesSem7to9 + votesSem1to6 + votesProbation;

        worksheet.Cells[2, 2].Value = votesSem7to9;
        worksheet.Cells[3, 2].Value = votesSem1to6;
        worksheet.Cells[4, 2].Value = votesProbation;
        worksheet.Cells[5, 2].Value = totalVotesAllGroups;
        var departments = lecturers.GroupBy(l => l.Department).ToList();
        var departmentRow = 3;

        foreach (var department in departments)
        {
            var voteCount = department.Sum(l => l.Votes);

            worksheet.Cells[$"M{departmentRow}"].Value = department.Key;
            worksheet.Cells[$"N{departmentRow}"].Value = voteCount;
            departmentRow++;
        }

        var accountDepartments = nonAdminAccounts
            .Where(a => !string.IsNullOrWhiteSpace(a.Department))
            .GroupBy(a => a.Department!)
            .ToList();
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

        worksheet.Cells["J21"].Value = nonAdminAccounts.Count;

        // Clear template highlight at cell B20
        worksheet.Cells[20, 2].Value = null;
        worksheet.Cells[20, 2].Style.Fill.PatternType = ExcelFillStyle.None;

        var range = worksheet.Cells[2, 1, 22, 14];
        ApplyTableFormatting(range);
    }

    /// <summary>
    ///     Fills data into Sheet 3: 'PARTICIPANTS' - Gmail, Lecturer name 1, 2, 3, ...
    /// </summary>
    private async Task FillParticipantsData(ExcelWorksheet worksheet, List<LecturerDto> lecturers,
        IReadOnlyList<AccountDto> accounts, CancellationToken cancellationToken)
    {
        // Build mapping: account email -> list of lecturer names voted (today)
        var nonAdminAccounts = accounts.Where(a => !a.IsAdmin).ToList();
        var emailToLecturerNames = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var lecturer in lecturers)
        {
            var votes = await lectureVoteService.GetAllVotesByLectureAsync(lecturer.Id, cancellationToken);
            foreach (var vote in votes)
            {
                var normalizedEmail = vote.Email.Trim().ToLowerInvariant();
                if (!nonAdminAccounts.Any(a => string.Equals(a.Email.Trim().ToLowerInvariant(), normalizedEmail, StringComparison.Ordinal)))
                    continue;
                if (!emailToLecturerNames.TryGetValue(normalizedEmail, out var list))
                {
                    list = new List<string>();
                    emailToLecturerNames[normalizedEmail] = list;
                }
                list.Add(lecturer.Name);
            }
        }

        var maxLecturersVoted = emailToLecturerNames.Values.DefaultIfEmpty([]).Max(l => l.Count);
        worksheet.Cells[1, 1].Value = "Gmail";
        for (var i = 1; i <= Math.Max(1, maxLecturersVoted); i++)
            worksheet.Cells[1, 1 + i].Value = $"Giáo viên {i}";

        const int startRow = 2;
        var currentRow = startRow;
        foreach (var account in nonAdminAccounts)
        {
            var normalizedEmail = account.Email.Trim().ToLowerInvariant();
            worksheet.Cells[currentRow, 1].Value = account.Email;
            if (emailToLecturerNames.TryGetValue(normalizedEmail, out var votedLecturers))
            {
                // Ensure stable, unique ordering
                var distinctLecturers = votedLecturers.Distinct().OrderBy(n => n).ToList();
                for (var j = 0; j < distinctLecturers.Count; j++)
                {
                    worksheet.Cells[currentRow, 2 + j].Value = distinctLecturers[j];
                }
            }
            currentRow++;
        }

        if (currentRow > startRow)
        {
            var lastCol = 1 + Math.Max(1, maxLecturersVoted);
            var range = worksheet.Cells[1, 1, currentRow - 1, lastCol];
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
        await FillDetailedStatsData(detailedStatsSheet, lecturers, feedbackVotes, accounts, cancellationToken);
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