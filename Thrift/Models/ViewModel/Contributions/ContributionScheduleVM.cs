using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel.Contributions
{
    public class ContributionScheduleVM
    {
        public ContributionScheduleVM()
        {

        }
        public ContributionScheduleVM(ContributionScheduleDTO row)
        {
            Id = row.Id;
            CustomerId = row.CustomerId;
            InitiatedContributionId = row.InitiatedContributionId;
            Paid = row.Paid;
            Cancel = row.Cancel;
            Amount = row.Amount;
            DateOfNextContribution = row.DateOfNextContribution;
            InitiatedContribution = new InitiatedContributionsVM(row.InitiatedContribution);
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int InitiatedContributionId { get; set; }
        public DateTime DateOfNextContribution { get; set; }
        public bool Paid { get; set; }
        public bool Cancel { get; set; }
        public decimal Amount { get; set; }

        public InitiatedContributionsVM InitiatedContribution;
    }
}