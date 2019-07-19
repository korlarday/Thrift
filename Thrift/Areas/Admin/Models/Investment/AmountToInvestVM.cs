using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift.Areas.Admin.Models.Investment
{
    public class AmountToInvestVM
    {
        public decimal AmountToInvest { get; set; }
        public decimal InvestmentBalance { get; set; }
    }
}