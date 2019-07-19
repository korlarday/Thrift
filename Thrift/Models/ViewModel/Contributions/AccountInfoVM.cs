using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.Data;
using Thrift.Models.ViewModel.Account;

namespace Thrift.Models.ViewModel.Contributions
{
    public class AccountInfoVM
    {
        public AccountInfoVM()
        {
        }

        public AccountInfoVM(AccountInfoDTO row)
        {
            Id = row.Id;
            CustomerId = row.CustomerId;
            BankName = row.BankName;
            AccountName = row.AccountName;
            AccountNumber = row.AccountNumber;
            Customer = new CustomerVM(row.Customer);
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }


        public CustomerVM Customer;
    }
}