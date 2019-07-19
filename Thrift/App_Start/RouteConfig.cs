using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Thrift
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Contributions", "Contributions/{action}/{slug}", new { controller = "Contributions", action = "Index", slug = UrlParameter.Optional }, new[] { "Thrift.Controllers" });
            routes.MapRoute("Account", "Account/{action}/{slug}", new { controller = "Account", action = "Index", slug = UrlParameter.Optional }, new[] { "Thrift.Controllers" });
            routes.MapRoute("Home", "Home/{action}/{slug}", new { controller = "Home", action = "Index", slug = UrlParameter.Optional }, new[] { "Thrift.Controllers" });
            routes.MapRoute("Default", "", new { controller = "Home", action = "Index" }, new[] { "Thrift.Controllers" });

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
