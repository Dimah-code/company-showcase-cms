using System.Web;
using System.Web.Mvc;
using PortoWeb.Helpers;

namespace PortoWeb.Helpers
{
    public class AuthorizeAdminAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var cookieValue = CookieHelper.GetDecryptedCookie("Aa", httpContext.Request);
            return cookieValue == "a";
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Admin/LoginPage");
        }
    }
}
