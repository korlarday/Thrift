using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblCustomerContributionSettings")]
    public class CustomerContributionSettingsDTO
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string ContributionMode { get; set; }
        public string DeductionMode { get; set; }

        [ForeignKey("CustomerId")]
        public virtual CustomerDTO Customer { get; set; }
    }
}