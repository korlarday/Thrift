using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel.Contributions
{
    public class InitiatedContributionsVM
    {
        public InitiatedContributionsVM()
        {
        }
        public InitiatedContributionsVM(InitiatedContributionsDTO row)
        {
            Id = row.Id; 
            CustomerId = row.CustomerId;
            DeductionMode = row.DeductionMode;
            ContributionMode = row.ContributionMode;
            ContributionAmount = row.ContributionAmount;
            ContributionDaysInterval = row.ContributionDaysInterval;
            Finished = row.Finished;
            TotalAmount = row.TotalAmount;
            StartDate = row.StartDate;
            EndDate = row.EndDate;
            Slug = row.Slug;
        }
        public int Id { get; set; }
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Deduction Mode")]
        public string DeductionMode { get; set; }

        [Required]
        [Display(Name = "Contribution Mode")]
        public string ContributionMode { get; set; }

        [Required]
        [Display(Name = "Contribution Amount")]
        public decimal ContributionAmount { get; set; }

        public int ContributionDaysInterval { get; set; }

        public bool Finished { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Slug { get; set; }

        public bool StartNew { get; set; }
        public bool Beneficiary { get; set; }

        public IEnumerable<SelectListItem> ContributionModeSelect { get; set; }
        public IEnumerable<SelectListItem> DeductionModeSelect { get; set; }
    }
}