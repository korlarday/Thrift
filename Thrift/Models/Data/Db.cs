using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    public class Db: DbContext
    {
        public DbSet<CustomerDTO> Customers { get; set; }
        public DbSet<CustomerContributionSettingsDTO> CustomersCSettings { get; set; }
        public DbSet<RolesDTO> Roles { get; set; }
        public DbSet<CustomerRoles> CustomerRoles { get; set; }
        public DbSet<AccountInfoDTO> AccountInfos { get; set; }
        public DbSet<BeneficiariesDTO> Beneficiaries { get; set; }
        public DbSet<NextOfKinDTO> NextOfKins { get; set; }
        public DbSet<ContributionScheduleDTO> ContributionSchedules { get; set; }
        public DbSet<ContributionProcessDTO> ContributionProcess { get; set; }
        public DbSet<InitiatedContributionsDTO> InitiatedContributions { get; set; }
        public DbSet<AccountContributionsDTO> AccountContributions { get; set; }
        public DbSet<AccountBalanceDTO> AccountBalance { get; set; }
        public DbSet<WithdrawalRequestDTO> WithdrawalRequests { get; set; }
        public DbSet<CustomerAllocationDTO> CustomerAllocations { get; set; }
        public DbSet<CustomerMessagesDTO> Messages { get; set; }
        // From the investment part
        public DbSet<CustomerInvestmentDTO> CustomerInvestments { get; set; }
        public DbSet<InvestmentBalanceDTO> InvestmentBalance { get; set; }
        public DbSet<ProfitPercentageDTO> ProfitPercentages { get; set; }
        public DbSet<ProfitDTO> Profits { get; set; }
        public DbSet<ProfitProcessDTO> ProfitProcesses { get; set; }
        public DbSet<ProfitProcessIBDTO> ProfitProcessIBs { get; set; }
    }
}