using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel.Investment
{
    public class ProfitProcessIBVM
    {
        public ProfitProcessIBVM()
        {

        }
        public ProfitProcessIBVM(ProfitProcessIBDTO row)
        {
            Id = row.Id;
            DateOfPayment = row.DateOfPayment;
            Amount = row.Amount;
            ProfitId = row.ProfitId;
        }

        public int Id { get; set; }
        public DateTime DateOfPayment { get; set; }
        public decimal Amount { get; set; }
        public int ProfitId { get; set; }
    }
}