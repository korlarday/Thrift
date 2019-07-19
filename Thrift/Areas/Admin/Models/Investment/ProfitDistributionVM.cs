using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.ViewModel.Investment;

namespace Thrift.Areas.Admin.Models.Investment
{
    public class ProfitDistributionVM
    {
        public List<ProfitProcessVM> Proccess { get; set; }
        public ProfitProcessIBVM ProfitProcessIB { get; set; }
        public decimal ProfitAmount { get; set; }
    }
}