using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblCustomerAllocation")]
    public class CustomerAllocationDTO
    {
        [Key]
        public int Id { get; set; }
        public int AdminId { get; set; }
        
        public int CustomerId { get; set; }

        [ForeignKey("AdminId")]
        public virtual CustomerDTO Admin { get; set; }

        [ForeignKey("CustomerId")]
        public virtual CustomerDTO Customer { get; set; }
    }
}