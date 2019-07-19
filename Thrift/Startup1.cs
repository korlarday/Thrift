using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Owin;
using Owin;
using Thrift.Controllers;
using Thrift.Models;

[assembly: OwinStartup(typeof(Thrift.Startup1))]

namespace Thrift
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            GlobalConfiguration.Configuration.UseSqlServerStorage(@"Server=(localdb)\MSSQLLocalDb;Integrated Security=true;AttachDbFileName=|DataDirectory|\Thrift.mdf;");
            app.UseHangfireDashboard("/myHangireDashboard", new DashboardOptions()
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            ContributionsController obj = new ContributionsController();
            RecurringJob.AddOrUpdate(() => obj.Recurrent(), Cron.Daily);
            app.UseHangfireServer();
        }
    }
}
