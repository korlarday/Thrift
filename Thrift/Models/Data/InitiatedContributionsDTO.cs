using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblInitiatedContributions")]
    public class InitiatedContributionsDTO
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string ContributionMode { get; set; }
        public string DeductionMode { get; set; }
        public decimal ContributionAmount { get; set; }
        public int ContributionDaysInterval { get; set; }
        public bool Finished { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Slug { get; set; }

        [ForeignKey("CustomerId")]
        public virtual CustomerDTO Customer { get; set; } 
    }
}