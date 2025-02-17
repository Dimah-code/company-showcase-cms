using Hangfire.Dashboard;
using System.Web;

namespace PortoWeb.Helpers
{
    public class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Access the current HTTP context
            var httpContext = HttpContext.Current;

            // Convert HttpRequest to HttpRequestBase
            var requestBase = new HttpRequestWrapper(httpContext.Request);

            // Use your helper to check if the user is an admin
            var cookieValue = CookieHelper.GetDecryptedCookie("Aa", requestBase);
            return cookieValue == "a"; // Only allow if the user is an admin
        }
    }
}
