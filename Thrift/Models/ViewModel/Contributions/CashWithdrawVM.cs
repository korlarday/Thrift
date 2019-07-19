using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Thrift.Models.ViewModel.Contributions
{
    public class CashWithdrawVM
    {
        [Display(Name = "Amount To Withdraw")]
        [Required(ErrorMessage = "You need to specify an amount to withdraw")]
        public decimal AmountToWithdraw { get; set; }

        public decimal AccountBalance { get; set; }
        public decimal ProfitBalance { get; set; }
        public bool withdrawalRequest { get; set; }
    }
}