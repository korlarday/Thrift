using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thrift.Models.ViewModel.Contributions;

namespace Thrift.Models.ViewModel.Account
{
    public class RegistrationVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Other Name(s)")]
        public string OtherName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode =true)]
        public DateTime DOB { get; set; }

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
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please upload your passport")]
        public HttpPostedFileBase Passport { get; set; }

        [Required(ErrorMessage = "Please upload your signature")]
        public HttpPostedFileBase Signature { get; set; }

        //Account Information
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        // Next of Kin
        [Required]
        [Display(Name = "Next of Kin Name")]
        public string NextOfKinName { get; set; }

        [Display(Name = "Next of Kin Email")]
        [EmailAddress]
        [Required]
        public string NextOfKinEmail { get; set; }

        [Required]
        [Display(Name = "Next of Kin Phone Number")]
        public string NextOfKinPhoneNumber { get; set; }

        [Display(Name = "Next of Kin Passort")]
        public HttpPostedFileBase NextOfKinPassport { get; set; }

        //Contribution Settings
        [Required]
        [Display(Name = "Contribution Mode")]
        public string ContributionMode { get; set; }
        [Required]
        [Display(Name = "Deduction Mode")]
        public string DeductionMode { get; set; }

        public string PassportName { get; set; }

        public List<BeneficiariesVM> Beneficiaries;
        public IEnumerable<SelectListItem> ContributionModeSelect { get; set; }
        public IEnumerable<SelectListItem> DeductionModeSelect { get; set; }
    }


    
}