using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.Data;
using Thrift.Models.ViewModel.Account;

namespace Thrift.Models.ViewModel.Investment
{
    public class ProfitProcessVM
    {
        public ProfitProcessVM()
        {

        }
        public ProfitProcessVM(ProfitProcessDTO row)
        {
            Id = row.Id;
            CustomerId = row.CustomerId;
            Date = row.Date;
            Amount = row.Amount;
            Percentage = row.Percentage;
            ProfitId = row.ProfitId;
            Customer = new CustomerVM(row.Customer);
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public int ProfitId { get; set; }

        public CustomerVM Customer { get; set; }
    }
}