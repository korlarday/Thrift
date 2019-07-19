using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thrift.Areas.Admin.Models;
using Thrift.Models.Data;
using Thrift.Models.ViewModel.Account;
using Thrift.Models.ViewModel.Contributions;

namespace Thrift.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    public class DashboardController : Controller
    {
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            DashboardVM model = new DashboardVM();
            using(Db db = new Db())
            {
                // fetch number of customers
                model.Customers = db.Customers.Count();

                // fetch the number of admins
                model.Admin = db.CustomerRoles.Where(x => x.RoleId == 2).Count();

                // Fetch the number of contributions made
                model.NumberOfContributions = db.ContributionProcess.Count();

                // Fetch the total number of withdrawals
                model.NumberOfWithdrawals = db.WithdrawalRequests.Where(x => x.Approve == true).Count();
                // Fetch the number of schedules available
                model.NumberOfSchedule = db.ContributionSchedules.Where(x => x.Paid == false && x.Cancel == false).Count();

                // Fetch account balance and amount withdrawn
                AccountBalanceDTO account = db.AccountBalance.Find(1);
                model.AccountBalance = account.AccountBalance;
                model.AmountWithdrawn = account.WithdrawalBalance;

                

                // fetch total amount of money invested
                model.AmountInvested = db.InvestmentBalance.Find(1).AccountBalance;
                // fetch available profit
                model.AvailableProfit = db.InvestmentBalance.Find(1).ProfitBalance;
            }
            return View(model);
        }

        // GET: Admin/Dashoard/UserInfoPartial
        public ActionResult UserInfoPartial()
        {
            // Get name
            string username = User.Identity.Name;

            // Declare model
            UserNavPartialVM model;

            using (Db db = new Db())
            {
                // Get the user
                CustomerDTO dto = db.Customers.SingleOrDefault(x => x.Username == username);

                // Build the model
                model = new UserNavPartialVM()
                {
                    Id = dto.Id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Username = dto.Username,
                    PassportName = dto.PassportName,
                    Email = dto.Email
                };
            }

            // Return partial view with model
            return PartialView(model);
        }

        // GET: Admin/Dashboard/NotificationPartial
        public ActionResult NotificationPartialAll()
        {
            // Declare a list of Request
            List<WithdrawalRequestVM> requests;

            using(Db db = new Db())
            {
                // Fetch requests
                requests = db.WithdrawalRequests.OrderByDescending(x => x.DateSent).Take(5).ToArray().Select(x => new WithdrawalRequestVM(x)).ToList();
            }

            return PartialView(requests);
        }

        public ActionResult NotificationPartial()
        {
            // Declare a list of Request
            List<WithdrawalRequestVM> requests;

            List<WithdrawalRequestVM> FilteredRequests = new List<WithdrawalRequestVM>();
            
            using (Db db = new Db())
            {
                if (User.IsInRole("All"))
                {
                    // Fetch requests
                    requests = db.WithdrawalRequests.OrderByDescending(x => x.DateSent).Take(5).ToArray().Select(x => new WithdrawalRequestVM(x)).ToList();
                    return PartialView(requests);
                }
                // fetch the admin
                string username = User.Identity.Name;
                CustomerDTO admin = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the users he is managing
                List<int> customers = db.CustomerAllocations.Where(x => x.AdminId == admin.Id).Select(x => x.CustomerId).ToList();

                // Fetch requests
                requests = db.WithdrawalRequests.OrderByDescending(x => x.DateSent).ToArray().Select(x => new WithdrawalRequestVM(x)).ToList();
                

                foreach (var item in requests)
                {
                    if (customers.Contains(item.CustomerId))
                    {
                        if(FilteredRequests.Count() < 6)
                        {
                            FilteredRequests.Add(item);
                        }
                    }
                }
            }

            return PartialView(FilteredRequests);
        }
    }
}