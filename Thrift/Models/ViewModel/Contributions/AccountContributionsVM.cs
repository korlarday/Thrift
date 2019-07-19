using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel.Contributions
{
    public class AccountContributionsVM
    {
        public AccountContributionsVM()
        {
        }

        public AccountContributionsVM(AccountContributionsDTO row)
        {
            Id = row.Id;
            CustomerId = row.CustomerId;
            ContributionsAccount = row.ContributionsAccount;
            WithdrawAccount = row.WithdrawAccount;
        }
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal ContributionsAccount { get; set; }
        public decimal? WithdrawAccount { get; set; }

        public bool AwaitingApproval { get; set; }
    }
}