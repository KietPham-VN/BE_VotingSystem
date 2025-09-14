using BE_VotingSystem.Infrastructure.Services;
using Hangfire;

namespace BE_VotingSystem.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for configuring recurring background jobs
/// </summary>
public static class RecurringJobExtensions
{
    /// <summary>
    /// Registers all recurring jobs using the service provider
    /// </summary>
    /// <param name="serviceProvider">The service provider to get services from</param>
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

    /// <summary>
    /// Registers the reset votes remain job with the recurring job manager
    /// </summary>
    /// <param name="recurringJobManager">The recurring job manager</param>
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

    /// <summary>
    /// Removes the reset votes remain job from the recurring job manager
    /// </summary>
    /// <param name="recurringJobManager">The recurring job manager</param>
    public static void RemoveResetVotesJob(this IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.RemoveIfExists("reset-votes-remain");
    }
}