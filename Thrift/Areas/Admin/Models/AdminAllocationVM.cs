using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.ViewModel.Account;

namespace Thrift.Areas.Admin.Models
{
    public class AdminAllocationVM
    {
        public AdminAndCustomersVM AdminAndCustomer { get; set; }
        public List<CustomerVM> UnAllocatedCustomers { get; set; }
    }
}