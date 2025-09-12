using BE_VotingSystem.Infrastructure.Services;
using Hangfire;

namespace BE_VotingSystem.Infrastructure.Extensions;

public static class RecurringJobExtensions
{
    public static void RegisterRecurringJobs(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        // Register reset votes remain job - chạy hàng ngày lúc 24h
        recurringJobManager.AddOrUpdate<IResetVotesService>(
            "reset-votes-remain",
            service => service.ResetVotesRemainAsync(),
            Cron.Daily(0, 0), // 24:00 (midnight)
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            });
    }

    public static void RegisterResetVotesJob(this IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate<IResetVotesService>(
            "reset-votes-remain",
            service => service.ResetVotesRemainAsync(),
            Cron.Daily(0, 0), // 24:00 (midnight)
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            });
    }

    public static void RemoveResetVotesJob(this IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.RemoveIfExists("reset-votes-remain");
    }
}