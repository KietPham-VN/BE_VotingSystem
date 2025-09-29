using BE_VotingSystem.Application.Dtos.Lecture;
using BE_VotingSystem.Application.Dtos.Lecture.Requests;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Domain.Entities;
using BE_VotingSystem.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Service implementation for lecturer management operations
/// </summary>
public class LecturerService(IAppDbContext context) : ILecturerService
{
    private const string CollationUtf8Mb4UnicodeCi = "utf8mb4_unicode_ci";
    private const string EmailField = "Email";
    /// <inheritdoc />
    public async Task<List<LecturerDto>> GetLecturers(
        bool? isActive = null, 
        SortBy sortBy = SortBy.Name, 
        OrderBy orderBy = OrderBy.Asc, 
        int? top = null, 
        CancellationToken cancellationToken = default)
    {
        var query = context.Lectures
            .AsNoTracking()
            .Include(l => l.Votes)
            .AsQueryable();

        if (isActive is not null)
            query = query.Where(l => l.IsActive == isActive);

        // Apply sorting
        query = sortBy switch
        {
            SortBy.Name => orderBy == OrderBy.Asc 
                ? query.OrderBy(l => l.Name)
                : query.OrderByDescending(l => l.Name),
            SortBy.Votes => orderBy == OrderBy.Asc 
                ? query.OrderBy(l => l.Votes.Count)
                : query.OrderByDescending(l => l.Votes.Count),
            SortBy.Department => orderBy == OrderBy.Asc 
                ? query.OrderBy(l => l.Department)
                : query.OrderByDescending(l => l.Department),
            SortBy.Email => orderBy == OrderBy.Asc 
                ? query.OrderBy(l => l.Email)
                : query.OrderByDescending(l => l.Email),
            _ => query.OrderBy(l => l.Name)
        };

        // Apply top limit if specified
        if (top.HasValue && top.Value > 0)
            query = query.Take(top.Value);

        return await query
            .Select(l => new LecturerDto(
                l.Id,
                l.Name ?? string.Empty,
                l.Email ?? string.Empty,
                l.Department ?? string.Empty,
                l.Quote ?? string.Empty,
                l.AvatarUrl ?? string.Empty,
                l.Votes.Count
            ))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Lecturer> AddLecturer(CreateLecturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingLecture = await context.Lectures
            .FirstOrDefaultAsync(
                l => l.Name != null && EF.Functions.Collate(l.Name, CollationUtf8Mb4UnicodeCi) == EF.Functions.Collate(request.Name, CollationUtf8Mb4UnicodeCi),
                cancellationToken);


        if (existingLecture is not null)
            throw new InvalidOperationException($"Lecturer with name '{request.Name}' already exists");

        var lecture = new Lecturer
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Department = request.Department.Trim(),
            Quote = request.Quote.Trim(),
            AvatarUrl = request.AvatarUrl.Trim()
        };

        context.Lectures.Add(lecture);
        await context.SaveChangesAsync(cancellationToken);

        return lecture;
    }

    /// <inheritdoc />
    public async Task<Lecturer> UpdateLecturer(Guid id, CreateLecturerRequest request,
        CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (lecture is null)
            throw new InvalidOperationException($"Lecturer with ID '{id}' not found");

        var existingLecture = await context.Lectures
            .FirstOrDefaultAsync(
                l => l.Id != id &&
                     l.Name != null &&
                     EF.Functions.Collate(l.Name, CollationUtf8Mb4UnicodeCi) == EF.Functions.Collate(request.Name, CollationUtf8Mb4UnicodeCi),
                cancellationToken);

        if (existingLecture is not null)
            throw new InvalidOperationException($"Lecturer with name '{request.Name}' already exists");

        lecture.Name = request.Name.Trim();
        lecture.Email = request.Email.Trim();
        lecture.Department = request.Department.Trim();
        lecture.Quote = request.Quote.Trim();
        lecture.AvatarUrl = request.AvatarUrl.Trim();

        await context.SaveChangesAsync(cancellationToken);
        return lecture;
    }

    /// <inheritdoc />
    public async Task DeleteLecturer(Guid id, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (lecture is null)
            throw new InvalidOperationException($"Lecturer with ID '{id}' not found");

        context.Lectures.Remove(lecture);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task ActivateLecturer(Guid id, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (lecture is null)
            throw new InvalidOperationException($"Lecturer with ID '{id}' not found");

        if (!lecture.IsActive)
        {
            lecture.IsActive = true;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task DeactivateLecturer(Guid id, CancellationToken cancellationToken = default)
    {
        var lecture = await context.Lectures.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        if (lecture is null)
            throw new InvalidOperationException($"Lecturer with ID '{id}' not found");

        if (lecture.IsActive)
        {
            lecture.IsActive = false;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<ImportLecturersResponse> ImportLecturersFromExcel(IFormFile file, CancellationToken cancellationToken = default)
    {
        var response = new ImportLecturersResponse();
        var lecturersToAdd = new List<Lecturer>();

        try
        {
            
            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            if (worksheet == null)
            {
                response.Errors.Add("Excel file does not contain any worksheets");
                return response;
            }

            var rowCount = worksheet.Dimension?.Rows ?? 0;
            if (rowCount < 2)
            {
                response.Errors.Add("Excel file must contain at least a header row and one data row");
                return response;
            }

            var headerValidation = ValidateHeaderRow(worksheet);
            if (!headerValidation.IsValid)
            {
                response.Errors.AddRange(headerValidation.Errors);
                return response;
            }

            var processedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var processedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            for (var i = 2; i <= rowCount; i++)
            {
                try
                {
                    var lecturerData = ExtractLecturerData(worksheet, i);
                    var validationResult = ValidateLecturerData(lecturerData, i);

                    if (!validationResult.IsValid)
                    {
                        response.RowErrors.AddRange(validationResult.RowErrors);
                        response.FailedCount++;
                        continue;
                    }

                    var isDuplicateInDb = await IsDuplicateLecturer(lecturerData, cancellationToken);
                    if (isDuplicateInDb)
                    {
                        response.RowErrors.Add(new RowError
                        {
                            RowNumber = i,
                            ErrorMessage = $"Lecturer with email '{lecturerData.Email}' already exists in database",
                            Field = EmailField
                        });
                        response.FailedCount++;
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(lecturerData.Email))
                    {
                        if (processedEmails.Contains(lecturerData.Email))
                        {
                            response.RowErrors.Add(new RowError
                            {
                                RowNumber = i,
                                ErrorMessage = $"Duplicate email '{lecturerData.Email}' found in current import batch",
                                Field = EmailField
                            });
                            response.FailedCount++;
                            continue;
                        }
                        processedEmails.Add(lecturerData.Email);
                    }

                    if (!string.IsNullOrWhiteSpace(lecturerData.Name))
                    {
                        if (processedNames.Contains(lecturerData.Name))
                        {
                            response.RowErrors.Add(new RowError
                            {
                                RowNumber = i,
                                ErrorMessage = $"Duplicate name '{lecturerData.Name}' found in current import batch",
                                Field = "Name"
                            });
                            response.FailedCount++;
                            continue;
                        }
                        processedNames.Add(lecturerData.Name);
                    }

                    var lecturer = new Lecturer
                    {
                        Id = Guid.NewGuid(),
                        AccountName = lecturerData.AccountName?.Trim(),
                        Name = lecturerData.Name?.Trim(),
                        Email = lecturerData.Email?.Trim(),
                        Department = lecturerData.Department?.Trim(),
                        Quote = lecturerData.Quote?.Trim(),
                        AvatarUrl = lecturerData.AvatarUrl?.Trim(),
                        IsActive = true
                    };

                    lecturersToAdd.Add(lecturer);
                    response.ImportedLecturers.Add(new LecturerImportResult
                    {
                        RowNumber = i,
                        AccountName = lecturer.AccountName ?? string.Empty,
                        Name = lecturer.Name ?? string.Empty,
                        Email = lecturer.Email ?? string.Empty,
                        Department = lecturer.Department ?? string.Empty
                    });
                }
                catch (Exception ex)
                {
                    response.RowErrors.Add(new RowError
                    {
                        RowNumber = i,
                        ErrorMessage = $"Error processing row: {ex.Message}",
                        Field = null
                    });
                    response.FailedCount++;
                }
            }

            if (lecturersToAdd.Any())
            {
                try
                {
                    context.Lectures.AddRange(lecturersToAdd);
                    await context.SaveChangesAsync(cancellationToken);
                    response.ImportedCount = lecturersToAdd.Count;
                }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException?.Message?.Contains("Duplicate entry") == true)
                        {
                            var individualSaveResults = await SaveLecturersIndividually(lecturersToAdd, cancellationToken);
                            response.ImportedCount = individualSaveResults.SuccessCount;
                            response.RowErrors.AddRange(individualSaveResults.Errors);
                            response.FailedCount += individualSaveResults.FailedCount;
                        }
                        else
                        {
                            throw;
                        }
                    }
            }

            response.IsSuccess = response.ImportedCount > 0;
            return response;
        }
        catch (Exception ex)
        {
            response.Errors.Add($"Error processing Excel file: {ex.Message}");
            return response;
        }
    }

    private static (bool IsValid, List<string> Errors) ValidateHeaderRow(ExcelWorksheet worksheet)
    {
        var errors = new List<string>();
        var requiredHeaders = new[] { "AccountName", "Name", "Email", "Department", "Quote", "AvatarUrl" };

        for (var col = 1; col <= requiredHeaders.Length; col++)
        {
            var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim();
            if (string.IsNullOrEmpty(headerValue) || !requiredHeaders.Contains(headerValue))
            {
                errors.Add($"Column {col} should be '{requiredHeaders[col - 1]}' but found '{headerValue}'");
            }
        }

        return (errors.Count == 0, errors);
    }

    private static LecturerData ExtractLecturerData(ExcelWorksheet worksheet, int row)
    {
        return new LecturerData
        {
            AccountName = worksheet.Cells[row, 1].Value?.ToString(),
            Name = worksheet.Cells[row, 2].Value?.ToString(),
            Email = worksheet.Cells[row, 3].Value?.ToString(),
            Department = worksheet.Cells[row, 4].Value?.ToString(),
            Quote = worksheet.Cells[row, 5].Value?.ToString(),
            AvatarUrl = worksheet.Cells[row, 6].Value?.ToString()
        };
    }

    private static (bool IsValid, List<RowError> RowErrors) ValidateLecturerData(LecturerData data, int rowNumber)
    {
        var errors = new List<RowError>();

        if (string.IsNullOrWhiteSpace(data.Name))
            errors.Add(new RowError { RowNumber = rowNumber, ErrorMessage = "Name is required", Field = "Name" });

        if (string.IsNullOrWhiteSpace(data.Email))
            errors.Add(new RowError { RowNumber = rowNumber, ErrorMessage = "Email is required", Field = EmailField });
        else if (!IsValidEmail(data.Email))
            errors.Add(new RowError { RowNumber = rowNumber, ErrorMessage = "Invalid email format", Field = EmailField });

        if (string.IsNullOrWhiteSpace(data.Department))
            errors.Add(new RowError { RowNumber = rowNumber, ErrorMessage = "Department is required", Field = "Department" });

        if (!string.IsNullOrWhiteSpace(data.Email) && !IsValidEmail(data.Email))
            errors.Add(new RowError { RowNumber = rowNumber, ErrorMessage = "Invalid email format", Field = EmailField });

        if (!string.IsNullOrWhiteSpace(data.AvatarUrl) && !IsValidUrl(data.AvatarUrl))
            errors.Add(new RowError { RowNumber = rowNumber, ErrorMessage = "Invalid URL format", Field = "AvatarUrl" });

        return (errors.Count == 0, errors);
    }

    private async Task<bool> IsDuplicateLecturer(LecturerData data, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(data.Email))
            return false;

        return await context.Lectures
            .AnyAsync(l => l.Email != null && 
                          EF.Functions.Collate(l.Email, CollationUtf8Mb4UnicodeCi) == 
                          EF.Functions.Collate(data.Email, CollationUtf8Mb4UnicodeCi), 
                      cancellationToken);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) && 
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    private async Task<(int SuccessCount, int FailedCount, List<RowError> Errors)> SaveLecturersIndividually(
        List<Lecturer> lecturers, CancellationToken cancellationToken)
    {
        var successCount = 0;
        var failedCount = 0;
        var errors = new List<RowError>();

        foreach (var lecturer in lecturers)
        {
                try
                {
                    context.Lectures.Add(lecturer);
                    await context.SaveChangesAsync(cancellationToken);
                    successCount++;
                }
            catch (DbUpdateException ex)
            {
                failedCount++;
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                
                errors.Add(new RowError
                {
                    RowNumber = 0,
                    ErrorMessage = $"Failed to save lecturer '{lecturer.Name}': {errorMessage}",
                    Field = null
                });
                
                if (context is DbContext dbContext)
                {
                    dbContext.Entry(lecturer).State = EntityState.Detached;
                }
            }
        }

        return (successCount, failedCount, errors);
    }

    private sealed class LecturerData
    {
        public string? AccountName { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? Quote { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
