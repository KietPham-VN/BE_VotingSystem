using BE_VotingSystem.Infrastructure.Services;
using Hangfire;

namespace BE_VotingSystem.Extensions;

/// <summary>
///     Provides extension methods for configuring recurring background jobs
/// </summary>
public static class RecurringJobExtensions
{
    /// <summary>
    ///     Registers all recurring jobs using the service provider
    /// </summary>
    /// <param name="serviceProvider">The service provider to get services from</param>
    public static void RegisterRecurringJobs(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<IResetVotesService>(
            "reset-votes-remain",
            service => service.ResetVotesRemainAsync(),
            Cron.Daily(0, 0),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            });
    }
}
