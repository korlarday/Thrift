using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblContributionSchedule")]
    public class ContributionScheduleDTO
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int InitiatedContributionId { get; set; }
        public DateTime DateOfNextContribution { get; set; }
        public bool Paid { get; set; }
        public bool Cancel { get; set; }
        public decimal Amount { get; set; }

        [ForeignKey("InitiatedContributionId")]
        public virtual InitiatedContributionsDTO InitiatedContribution { get; set; }
    }
}