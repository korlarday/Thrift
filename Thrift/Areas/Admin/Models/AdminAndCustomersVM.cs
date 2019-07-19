using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.ViewModel.Account;

namespace Thrift.Areas.Admin.Models
{
    public class AdminAndCustomersVM
    {
        public CustomerVM Admin { get; set; }
        public List<CustomerAllocationVM> Customers { get; set; }
    }
}