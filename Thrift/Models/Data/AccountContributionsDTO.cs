using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblContributionsAccount")]
    public class AccountContributionsDTO
    {
        [Key]
        public int Id { get; set; }
        public decimal ContributionsAccount { get; set; }
        public decimal? WithdrawAccount { get; set; }
        public int CustomerId { get; set; }
    }
}