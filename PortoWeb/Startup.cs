using System;
using Hangfire;
using Microsoft.Owin;
using Owin;
using PortoWeb.Models;
using PortoWeb.Controllers;
using PortoWeb.Services;
using PortoWeb.Helpers;

[assembly: OwinStartup(typeof(PortoWeb.Startup))]
namespace PortoWeb
{
    public class Startup
    {
        [Obsolete]
        public void Configuration(IAppBuilder app)
        {
            string sqlServerConnectionString = "data source=zeusshoes.shop;initial catalog=23974_Porto;persist security info=True;user id=23974;password=d@14QeYNgvjvXR#y;MultipleActiveResultSets=True;";
            GlobalConfiguration.Configuration.UseSqlServerStorage(sqlServerConnectionString);

            // Hangfire Dashboard with restricted access
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAdminAuthorizationFilter() }
            });

            var options = new BackgroundJobServerOptions
            {
                WorkerCount = Environment.ProcessorCount * 2
            };
            app.UseHangfireServer(options);
          

            // Schedule recurring job
            RecurringJob.AddOrUpdate<MessageService>(service => service.DeleteOldMessages(), Cron.Daily);
        }
    }
}
