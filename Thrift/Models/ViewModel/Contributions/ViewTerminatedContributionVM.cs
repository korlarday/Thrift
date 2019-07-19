using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift.Models.ViewModel.Contributions
{
    public class ViewTerminatedContributionVM
    {
        public InitiatedContributionsVM InitiatedContribution { get; set; }
        public List<ContributionProcessVM> Processes { get; set; }

        public string Username { get; set; }
    }
}