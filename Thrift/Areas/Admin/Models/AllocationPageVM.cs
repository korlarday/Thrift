using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.ViewModel.Account;

namespace Thrift.Areas.Admin.Models
{
    public class AllocationPageVM
    {
        public List<AdminAndCustomersVM> AdminAndCustomers { get; set; }
        public List<CustomerVM> UnAllocatedCustomers { get; set; }
    }
}