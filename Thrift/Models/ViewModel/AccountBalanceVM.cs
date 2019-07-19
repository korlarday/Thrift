using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel
{
    public class AccountBalanceVM
    {
        public AccountBalanceVM()
        {

        }
        public AccountBalanceVM(AccountBalanceDTO row)
        {
            Id = row.Id;
            AccountBalance = row.AccountBalance;
            WithdrawalBalance = row.WithdrawalBalance;
        }

        public int Id { get; set; }
        [Display(Name = "Account Balance")]
        public decimal AccountBalance { get; set; }
        public decimal WithdrawalBalance { get; set; }
    }
}