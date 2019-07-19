using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift.Models
{
    public class HangfireAuthorizationFilter: IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            //throw new NotImplementedException();
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }
    }
}