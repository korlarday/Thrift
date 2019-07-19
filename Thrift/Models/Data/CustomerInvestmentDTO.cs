using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblCustomerInvestment")]
    public class CustomerInvestmentDTO
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal InvestmentBalance { get; set; }
        public decimal AvailableProfit { get; set; }
        public decimal ProfitWithdrawn { get; set; }
    }
}