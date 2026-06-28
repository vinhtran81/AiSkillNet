using Hangfire.Dashboard;

namespace SkillNet.Web.Infrastructure;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();
        return http.User.IsInRole("Admin");
    }
}
