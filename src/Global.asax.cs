using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Hangfire;
using System.Linq;
using Hangfire.SqlServer;
using PortoWeb.Controllers;
using PortoWeb.Models;
using System.Data.Entity;

namespace PortoWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        [Obsolete]
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
        // Check for the language cookie at the beginning of each request
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var langCookie = HttpContext.Current.Request.Cookies["lang"];
            var currentUrl = HttpContext.Current.Request.Url.AbsolutePath;

            // Check if the cookie is missing and the request is NOT for the SelectLanguage page
            if (langCookie == null
                && !currentUrl.StartsWith("/Home/SelectLanguage", StringComparison.OrdinalIgnoreCase)
                && !currentUrl.StartsWith("/Home/SetLanguage", StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Current.Response.Redirect("~/Home/SelectLanguage");
            }
        }

    }
}
