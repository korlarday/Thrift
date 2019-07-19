using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.ViewModel.Investment;

namespace Thrift.Models.ViewModel.Contributions
{
    public class DashboardModelVM
    {
        public AccountContributionsVM Account { get; set; }
        public ContributionScheduleVM ContributionSchedule { get; set; }
        public List<BeneficiariesVM> Beneficiaries { get; set; }
        public CustomerInvestmentVM InvestmentAccount { get; set; }
    }
}