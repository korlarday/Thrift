using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel.Investment
{
    public class InvestmentBalanceVM
    {
        public InvestmentBalanceVM()
        {

        }
        public InvestmentBalanceVM(InvestmentBalanceDTO row)
        {
            Id = row.Id;
            AccountBalance = row.AccountBalance;
            ProfitBalance = row.ProfitBalance;
        }

        public int Id { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal ProfitBalance { get; set; }
    }
}