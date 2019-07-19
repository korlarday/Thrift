using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift.Models.ViewModel
{
    public class StateData
    {
        public string State { get; set; }
        public string Alias { get; set; }
        public string[] Lgas { get; set; }
    }
}