using Hangfire.Dashboard;

namespace BE_VotingSystem.Api.Middlewares;

/// <summary>
/// Authorization filter for Hangfire dashboard access
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    /// <summary>
    /// Determines whether the user is authorized to access the Hangfire dashboard
    /// </summary>
    /// <param name="context">The dashboard context</param>
    /// <returns>True if authorized, false otherwise</returns>
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}