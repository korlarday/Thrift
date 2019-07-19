using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift.Areas.Admin.Models.Investment
{
    public class SelectAvailableCashVM
    {
        public decimal CurrentInvestBalance { get; set; }
        public List<CustomerAndAmount> Customers { get; set; }
    }
}