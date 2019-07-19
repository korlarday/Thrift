using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Thrift.Models.Data;
using Thrift.Models.ViewModel.Account;

namespace Thrift.Models.ViewModel.Contributions
{
    public class WithdrawalRequestVM
    {
        public WithdrawalRequestVM()
        {

        }
        public WithdrawalRequestVM(WithdrawalRequestDTO row)
        {
            Id = row.Id;
            CustomerId = row.CustomerId;
            Amount = row.Amount;
            DateSent = row.DateSent;
            AdminStatus = row.AdminStatus;
            Slug = row.Slug;
            RespondBy = row.RespondBy;
            Customer = new CustomerVM(row.Customer);
            Approve = row.Approve;
            AccountBalance = row.AccountBalance;
            AvailableProfit = row.AvailableProfit;
            if(row.RespondBy != null)
                Responder = new CustomerVM(row.Responder);
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateSent { get; set; }
        public bool AdminStatus { get; set; }
        public string Slug { get; set; }
        public int? RespondBy { get; set; }
        public bool Approve { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal AvailableProfit { get; set; }

        public CustomerVM Customer { get; set; }
        public CustomerVM Responder { get; set; }


    }
}