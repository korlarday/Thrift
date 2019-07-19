using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel.Investment
{
    public class ProfitVM
    {
        public ProfitVM()
        {

        }
        public ProfitVM(ProfitDTO row)
        {
            Id = row.Id;
            DateOfPayment = row.DateOfPayment;
            Amount = row.Amount;
            Slug = row.Slug;
        }

        public int Id { get; set; }
        public DateTime DateOfPayment { get; set; }
        public decimal Amount { get; set; }
        public string Slug { get; set; }
    }
}