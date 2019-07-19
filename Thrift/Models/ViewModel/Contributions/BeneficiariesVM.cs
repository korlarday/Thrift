using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel.Contributions
{
    public class BeneficiariesVM
    {
        public BeneficiariesVM()
        {

        }
        public BeneficiariesVM(BeneficiariesDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Email = row.Email;
            PhoneNumber = row.PhoneNumber;
            CustomerId = row.CustomerId;
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }
        public int CustomerId { get; set; }
    }
}