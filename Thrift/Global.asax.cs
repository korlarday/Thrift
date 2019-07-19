using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Thrift.Models.Data;

namespace Thrift
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest()
        {
            // check if user is logged in
            if (User == null) { return; }

            // Get username
            string username = Context.User.Identity.Name;

            // Declare array of roles
            string[] roles = null;

            using (Db db = new Db())
            {
                // Populate roles
                CustomerDTO dto = db.Customers.FirstOrDefault(x => x.Username == username);

                roles = db.CustomerRoles.Where(x => x.CustomerId == dto.Id).Select(x => x.Role.Name).ToArray();

            }

            // Build IPrincipal object
            IIdentity userIdentity = new GenericIdentity(username);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            // Update Contex.User
            Context.User = newUserObj;
        }
    }
}
