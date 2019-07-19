using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblRoles")]
    public class RolesDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}