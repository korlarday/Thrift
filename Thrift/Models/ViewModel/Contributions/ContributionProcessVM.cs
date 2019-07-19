using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.Data;
using Thrift.Models.ViewModel.Account;

namespace Thrift.Models.ViewModel.Contributions
{
    public class ContributionProcessVM
    {
        public ContributionProcessVM()
        {
        }

        public ContributionProcessVM(ContributionProcessDTO row)
        {
            Id = row.Id;
            InitiatedContributionsId = row.InitiatedContributionsId; 
            CustomerId = row.CustomerId;
            AmountPaid = row.AmountPaid;
            TimeOfContribution = row.TimeOfContribution;
            ContributionType = row.ContributionType;
            DeductionMode = row.DeductionMode;
            Invested = row.Invested;
            Customer = new CustomerVM(row.Customer);
        }

        public int Id { get; set; }
        public int InitiatedContributionsId { get; set; }
        public int CustomerId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime TimeOfContribution { get; set; }
        public string ContributionType { get; set; }
        public string DeductionMode { get; set; }
        public bool Invested { get; set; }

        public CustomerVM Customer { get; set; }
    }
}