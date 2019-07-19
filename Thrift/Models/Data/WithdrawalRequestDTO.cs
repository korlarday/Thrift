using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblWithdrawalRequest")]
    public class WithdrawalRequestDTO
    {
        [Key]
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

        [ForeignKey("CustomerId")]
        public virtual CustomerDTO Customer { get; set; }

        [ForeignKey("RespondBy")]
        public virtual CustomerDTO Responder { get; set; }
    }
}