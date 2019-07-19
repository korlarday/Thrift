using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel.Investment
{
    public class ProfitPercentageVM
    {
        public ProfitPercentageVM()
        {

        }
        public ProfitPercentageVM(ProfitPercentageDTO row)
        {
            Id = row.Id;
            CustomerId = row.CustomerId;
            Percentage = row.Percentage;
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Percentage { get; set; }
    }
}