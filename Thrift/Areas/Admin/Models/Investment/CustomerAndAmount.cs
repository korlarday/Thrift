using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thrift.Areas.Admin.Models.Investment
{
    public class CustomerAndAmount
    {
        public string Username { get; set; }
        public int Id { get; set; }
        public decimal AmountAvailableToInvest { get; set; }
    }
}