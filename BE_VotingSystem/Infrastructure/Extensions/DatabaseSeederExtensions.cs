using BE_VotingSystem.Infrastructure.Database;

namespace BE_VotingSystem.Infrastructure.Extensions;

/// <summary>
///     Extension methods for database seeding
/// </summary>
public static class DatabaseSeederExtensions
{
    /// <summary>
    ///     Seeds the database with sample data
    /// </summary>
    /// <param name="app">The web application</param>
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        await DbSeeder.SeedAsync(context);
    }
}
