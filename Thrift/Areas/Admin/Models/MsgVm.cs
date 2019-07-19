using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift.Areas.Admin.Models
{
    public class MsgVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}