using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Thrift.Models.Data
{
    [Table("tblCustomerRoles")]
    public class CustomerRoles
    {
        [Key, Column(Order = 0)]
        public int CustomerId { get; set; }

        [Key, Column(Order = 1)]
        public int RoleId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual CustomerDTO Customer { get; set; }
        [ForeignKey("RoleId")]
        public virtual RolesDTO Role { get; set; }
    }
}