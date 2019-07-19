using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblProfitPercentage")]
    public class ProfitPercentageDTO
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Percentage { get; set; }
    }
}