using System.Web.Mvc;

namespace Thrift.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Investments",
                "Admin/Investments/{action}/{slug}",
                new { controller = "Investments", action = "Index", slug = UrlParameter.Optional },
                new[] { "Thrift.Areas.Admin.Controllers" });
            context.MapRoute(
                "Transactions",
                "Admin/Transactions/{action}/{slug}",
                new { controller = "Transactions", action = "Personal", slug = UrlParameter.Optional },
                new[] { "Thrift.Areas.Admin.Controllers" });
            context.MapRoute(
                "Customers", 
                "Admin/Customers/{action}/{slug}", 
                new { controller = "Customers", action = "Index", slug = UrlParameter.Optional }, 
                new[] { "Thrift.Areas.Admin.Controllers" });
            context.MapRoute("Dashboard", "Admin/Dashboard/{action}/{slug}", new { controller = "Dashboard", action = "Index", slug = UrlParameter.Optional }, new[] { "Thrift.Areas.Admin.Controllers" });

            //context.MapRoute(
            //    "Admin_default",
            //    "Admin/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}