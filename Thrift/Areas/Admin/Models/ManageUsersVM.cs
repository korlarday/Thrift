using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.ViewModel.Account;

namespace Thrift.Areas.Admin.Models
{
    public class ManageUsersVM
    {
        public List<CustomerVM> Administrators { get; set; }
        public List<CustomerVM> Nonadministrators { get; set; }
    }
}