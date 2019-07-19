using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Thrift.Models.Data;
using Thrift.Models.ViewModel.Contributions;
using Thrift.Models.ViewModel.Investment;

namespace Thrift.Models.ViewModel.Account
{
    public class CustomerVM
    {
        public CustomerVM()
        {
        }

        public CustomerVM(CustomerDTO row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            OtherName = row.OtherName;
            Address = row.Address;
            DOB = row.DOB;
            PhoneNumber = row.PhoneNumber;
            StateOfOrigin = row.StateOfOrigin;
            LGA = row.LGA;
            Town = row.Town;
            Username = row.Username;
            Password = row.Password;
            Email = row.Email;
            PassportName = row.PassportName;
            AccountId = row.AccountId;
            Account = new AccountContributionsVM(row.Account);
        }

        public int Id { get; set; }

        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Other Name(s)")]
        public string OtherName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime? DOB { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "State Of Origin")]
        public string StateOfOrigin { get; set; }

        [Required]
        public string LGA { get; set; }

        public string Town { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please upload your passport")]
        public string PassportName { get; set; }

        [Required(ErrorMessage ="Please upload your signature")]
        public string SignatureName { get; set; }
        public int? AccountId { get; set; }


        public ContributionScheduleVM OngoingContributions;
        public List<InitiatedContributionsVM> TerminatedContributions { get; set; }
        public AccountContributionsVM Account { get; set; }
        public CustomerInvestmentVM Investment { get; set; }

    }
}