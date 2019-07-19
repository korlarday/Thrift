using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift.Areas.Admin.Models
{
    public class DashboardVM
    {
        public int Customers { get; set; }
        public int Admin { get; set; }
        public int NumberOfContributions { get; set; }
        public int NumberOfWithdrawals { get; set; }
        public int NumberOfSchedule { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal AmountInvested { get; set; }
        public decimal AvailableProfit { get; set; }
        public decimal AmountWithdrawn { get; set; }
    }
}