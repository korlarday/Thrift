using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift.Models.ViewModel.Contributions
{
    public class OngoingContributionVM
    {
        public InitiatedContributionsVM InitiatedContributionVM { get; set; }
        public AccountContributionsVM AccountVM { get; set; }
        public List<ContributionProcessVM> ProcessVMs { get; set; }
        public ContributionScheduleVM ContributionScheduleVM { get; set; }
        public string Username { get; set; }
    }
}