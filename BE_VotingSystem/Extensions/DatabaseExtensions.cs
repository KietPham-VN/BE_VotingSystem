using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Infrastructure.Database;
using BE_VotingSystem.Infrastructure.Setting;

namespace BE_VotingSystem.Extensions;

/// <summary>
///     Provides extension methods for configuring database services
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    ///     Adds database configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(
            configuration.GetSection(DatabaseSettings.SectionName));

        var dbSettings = configuration
            .GetSection(DatabaseSettings.SectionName)
            .Get<DatabaseSettings>();

        if (dbSettings == null || string.IsNullOrEmpty(dbSettings.DefaultConnection))
            throw new InvalidOperationException("Missing connection string in appsettings.json");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                dbSettings.DefaultConnection,
                ServerVersion.AutoDetect(dbSettings.DefaultConnection)
            )
        );

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }
}