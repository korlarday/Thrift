using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblCustomer")]
    public class CustomerDTO
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string Address { get; set; }
        public DateTime DOB { get; set; }
        public string PhoneNumber { get; set; }
        public string StateOfOrigin { get; set; }
        public string LGA { get; set; }
        public string Town { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string SignatureName { get; set; }
        public string PassportName { get; set; }
        public int? AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual AccountContributionsDTO Account { get; set; }
    }
}