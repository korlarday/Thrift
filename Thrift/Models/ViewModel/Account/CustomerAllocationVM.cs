using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel.Account
{
    public class CustomerAllocationVM
    {
        public CustomerAllocationVM()
        {

        }

        public CustomerAllocationVM(CustomerAllocationDTO row)
        {
            Id = row.Id;
            AdminId = row.AdminId;
            CustomerId = row.CustomerId;

            Admin = new CustomerVM(row.Admin);
            Customer = new CustomerVM(row.Customer);
        }
        public int Id { get; set; }
        public int AdminId { get; set; }
        public int CustomerId { get; set; }

        public CustomerVM Admin { get; set; }
        public CustomerVM Customer { get; set; }
    }
}