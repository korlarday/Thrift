using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift.Areas.Admin.Models
{
    public class SetAuthorization
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool Admin { get; set; }
        public bool Allocation { get; set; }
        public bool All { get; set; }
        public bool Investment { get; set; }
    }
}