using BE_VotingSystem.Infrastructure.Setting;
using Hangfire;
using Hangfire.MySql;

namespace BE_VotingSystem.Extensions;

/// <summary>
///     Provides extension methods for configuring Hangfire background job services
/// </summary>
public static class HangfireExtensions
{
    /// <summary>
    ///     Adds Hangfire configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddHangfireConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbSettings = configuration
            .GetSection(DatabaseSettings.SectionName)
            .Get<DatabaseSettings>();

        if (dbSettings == null || string.IsNullOrEmpty(dbSettings.DefaultConnection))
            throw new InvalidOperationException("Missing connection string in appsettings.json");

        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(
                new MySqlStorage(
                    dbSettings.DefaultConnection,
                    new MySqlStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(10),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 25000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "Hangfire"
                    }
                )
            ));

        services.AddHangfireServer(options => options.WorkerCount = 1);

        return services;
    }
}