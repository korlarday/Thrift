using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblContributionProcess")]
    public class ContributionProcessDTO
    {
        [Key]
        public int Id { get; set; }
        public int InitiatedContributionsId { get; set; }
        public int CustomerId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime TimeOfContribution { get; set; }
        public string ContributionType { get; set; }
        public string DeductionMode { get; set; }
        public bool Invested { get; set; }

        [ForeignKey("CustomerId")]
        public virtual CustomerDTO Customer { get; set; }
    }
}