using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel.Investment
{
    public class CustomerInvestmentVM
    {
        public CustomerInvestmentVM()
        {

        }
        public CustomerInvestmentVM(CustomerInvestmentDTO row)
        {
            Id = row.Id;
            CustomerId = row.CustomerId;
            InvestmentBalance = row.InvestmentBalance;
            AvailableProfit = row.AvailableProfit;
            ProfitWithdrawn = row.ProfitWithdrawn;
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal InvestmentBalance { get; set; }
        public decimal AvailableProfit { get; set; }
        public decimal ProfitWithdrawn { get; set; }
    }
}