using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Thrift.Models.ViewModel.Account
{
    public class LoginVM
    {
        [Required]
        [Display(Name ="Username:")]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Password:")]
        public string Password { get; set; }
        public string RememberMe { get; set; }
    }
}