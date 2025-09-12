using Hangfire.Dashboard;

namespace BE_VotingSystem.Api.Middlewares;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // Trong development, cho phép tất cả
        if (context.GetHttpContext().RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true)
        {
            return true;
        }

        // Trong production, có thể thêm logic kiểm tra role/permission
        // Ví dụ: kiểm tra JWT token hoặc role admin
        var httpContext = context.GetHttpContext();
        
        // Kiểm tra authentication
        if (!httpContext.User.Identity?.IsAuthenticated ?? true)
        {
            return false;
        }

        
        return true; 
    }
}
