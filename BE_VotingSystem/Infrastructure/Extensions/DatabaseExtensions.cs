using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Infrastructure.Database;
using BE_VotingSystem.Infrastructure.Setting;

namespace BE_VotingSystem.Infrastructure.Extensions;

public static class DatabaseExtensions
{
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
