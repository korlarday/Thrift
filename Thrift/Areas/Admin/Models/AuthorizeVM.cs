using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Thrift.Models.ViewModel.Account;

namespace Thrift.Areas.Admin.Models
{
    public class AuthorizeVM
    {
        public CustomerVM Customer { get; set; }
        [Display(Name = "Administrator")]
        public bool Admin { get; set; }
        [Display(Name = "Customer Allocations")]
        public bool Allocation { get; set; }
        [Display(Name = "Super Administrator")]
        public bool All { get; set; }
        public bool Investment { get; set; }
    }
}