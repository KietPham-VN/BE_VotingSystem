using BE_VotingSystem.Domain.Entities;
using BE_VotingSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BE_VotingSystem.Infrastructure.Database;

/// <summary>
///     Database seeder for populating the database with sample data
/// </summary>
public static class DbSeeder
{
    // Lecturer departments
    private static readonly string[] LecturerDepartments = 
    [
        "Bộ môn Tiếng Anh dự bị",
        "Bộ môn Âm nhạc Truyền thống",
        "Bộ môn Kỹ năng mềm",
        "Bộ môn Chính trị",
        "Bộ môn Toán",
        "Bộ môn Giáo dục thể chất",
        "Bộ môn Kỹ thuật phần mềm",
        "Bộ môn Hoạt hình kỹ thuật số",
        "Bộ môn Quản trị Truyền thông đa phương tiện",
        "Bộ môn tiếng Nhật",
        "Bộ môn Quản trị doanh nghiệp",
        "Bộ môn Thiết kế đồ họa",
        "Bộ môn Quản trị Du lịch và khách sạn",
        "Bộ môn CF",
        "Bộ môn Phát triển khởi nghiệp",
        "Bộ môn Tiếng Anh chuyên ngành",
        "Bộ môn An toàn thông tin",
        "Bộ môn ITS"
    ];

    // Account departments
    private static readonly string[] AccountDepartments = 
    [
        "Kỹ thuật phần mềm",
        "An toàn thông tin",
        "Trí tuệ nhân tạo",
        "Ngôn ngữ Anh",
        "Ngôn ngữ Nhật",
        "Thiết kế Mỹ thuật số",
        "Quản trị Truyền thông Đa phương tiện",
        "Quản trị Khách sạn",
        "Hệ thống thông tin",
        "Quản trị Dịch vụ Du lịch & Lữ hành",
        "Digital Marketing",
        "Kinh Doanh Quốc tế",
        "Tài chính"
    ];

    private const string AvatarBaseUrl = "https://via.placeholder.com/150";

    /// <summary>
    ///     Seeds the database with sample data
    /// </summary>
    /// <param name="context">The database context</param>
    public static async Task SeedAsync(AppDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        var hasData = await context.Accounts.AnyAsync();
        if (hasData)
        {
            Console.WriteLine("Database already contains data. Skipping seed.");
            return; // Data already seeded
        }

        Console.WriteLine("Starting database seeding...");

        // Seed Accounts
        var accounts = await SeedAccountsAsync(context);

        // Seed Lecturers
        var lecturers = await SeedLecturersAsync(context);

        // Seed LecturerVotes
        await SeedLecturerVotesAsync(context, accounts, lecturers);

        // Seed FeedbackVotes
        await SeedFeedbackVotesAsync(context, accounts);

        await context.SaveChangesAsync();
        Console.WriteLine("Database seeding completed successfully!");
    }

    private static async Task<List<Account>> SeedAccountsAsync(AppDbContext context)
    {
        var random = new Random();
        var accounts = new List<Account>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Email = "admin@voting.com",
                Name = "Admin User",
                StudentCode = "SS000001",
                Semester = 9,
                Department = "Kỹ thuật phần mềm",
                IsAdmin = true,
                IsBanned = false,
                Provider = AuthProvider.Local,
                ProviderId = "admin_local",
                VotesRemain = 3
            }
        };

        // Generate 9 more accounts with random departments
        for (var i = 1; i < 10; i++)
        {
            var department = AccountDepartments[random.Next(AccountDepartments.Length)];
            var isBanned = i == 9; // Last account is banned
            
            // Generate valid StudentCode format: SS/SA/SE/CS/CA/CE/HS/HE/HA/QS/QA/QE/DS/DA/DE + 6 digits
            var prefixes = new[] { "SS", "SA", "SE", "CS", "CA", "CE", "HS", "HE", "HA", "QS", "QA", "QE", "DS", "DA", "DE" };
            var prefix = prefixes[random.Next(prefixes.Length)];
            var studentCode = $"{prefix}{random.Next(100000, 999999)}";
            
            accounts.Add(new Account
            {
                Id = Guid.NewGuid(),
                Email = $"student{i}@university.edu",
                Name = $"Student {i}",
                StudentCode = studentCode,
                Semester = (byte)random.Next(1, 10),
                Department = department,
                IsAdmin = false,
                IsBanned = isBanned,
                BanReason = isBanned ? "Violation of voting rules" : null,
                Provider = AuthProvider.Google,
                ProviderId = $"google_{i:D9}",
                VotesRemain = isBanned ? (byte)0 : (byte)random.Next(1, 4)
            });
        }

        await context.Accounts.AddRangeAsync(accounts);
        Console.WriteLine($"Seeded {accounts.Count} accounts");
        return accounts;
    }

    private static async Task<List<Lecturer>> SeedLecturersAsync(AppDbContext context)
    {
        var random = new Random();
        var lecturers = new List<Lecturer>();

        // Generate 10 lecturers with random departments
        for (var i = 0; i < 10; i++)
        {
            var department = LecturerDepartments[random.Next(LecturerDepartments.Length)];
            var isActive = i < 8; // First 8 are active, last 2 are inactive
            
            lecturers.Add(new Lecturer
            {
                Id = Guid.NewGuid(),
                Name = $"Giảng viên {i + 1}",
                Email = $"lecturer{i + 1}@university.edu",
                Department = department,
                Quote = $"Quote của giảng viên {i + 1}",
                AvatarUrl = $"{AvatarBaseUrl}?text=GV{i + 1}",
                IsActive = isActive
            });
        }

        await context.Lectures.AddRangeAsync(lecturers);
        Console.WriteLine($"Seeded {lecturers.Count} lecturers");
        return lecturers;
    }

    private static async Task SeedLecturerVotesAsync(AppDbContext context, List<Account> accounts, List<Lecturer> lecturers)
    {
        var random = new Random();
        var votes = new List<LecturerVote>();
        var activeLecturers = lecturers.Where(l => l.IsActive).ToList();
        var activeAccounts = accounts.Where(a => !a.IsBanned).ToList();
        var usedCombinations = new HashSet<string>();

        // Generate 100 votes
        for (var i = 0; i < 100; i++)
        {
            var account = activeAccounts[random.Next(activeAccounts.Count)];
            var lecturer = activeLecturers[random.Next(activeLecturers.Count)];
            var votedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-random.Next(0, 30)));
            
            // Create unique key to avoid duplicates
            var uniqueKey = $"{lecturer.Id}-{account.Id}-{votedDate:yyyy-MM-dd}";
            
            // Skip if this combination already exists
            if (usedCombinations.Contains(uniqueKey))
            {
                i--; // Try again
                continue;
            }
            
            usedCombinations.Add(uniqueKey);

            votes.Add(new LecturerVote
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                LectureId = lecturer.Id,
                VotedAt = votedDate
            });
        }

        await context.LectureVotes.AddRangeAsync(votes);
        Console.WriteLine($"Seeded {votes.Count} lecturer votes");
    }

    private static async Task SeedFeedbackVotesAsync(AppDbContext context, List<Account> accounts)
    {
        var random = new Random();
        var feedbackVotes = new List<FeedbackVote>();

        // Generate 10 feedback votes (one per account, but only for first 10 accounts)
        for (var i = 0; i < Math.Min(10, accounts.Count); i++)
        {
            var account = accounts[i];
            
            feedbackVotes.Add(new FeedbackVote
            {
                AccountId = account.Id,
                Vote = random.Next(1, 6), // Random vote between 1-5
                VotedAt = DateTime.UtcNow.AddDays(-random.Next(0, 7)) // Random date within last week
            });
        }

        await context.FeedbackVotes.AddRangeAsync(feedbackVotes);
        Console.WriteLine($"Seeded {feedbackVotes.Count} feedback votes");
    }
}