using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblProfitProcess")]
    public class ProfitProcessDTO
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public int ProfitId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual CustomerDTO Customer { get; set; }
    }
}