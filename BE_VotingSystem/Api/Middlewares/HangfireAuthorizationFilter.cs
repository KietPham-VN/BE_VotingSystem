using Hangfire.Dashboard;

namespace BE_VotingSystem.Api.Middlewares;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}