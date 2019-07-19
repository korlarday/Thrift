using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thrift.Models.Data;
using Thrift.Models.ViewModel.Account;
using Thrift.Models.ViewModel.Contributions;
using PagedList;
using Thrift.Models.ViewModel.Investment;

namespace Thrift.Controllers
{
    [Authorize]
    public class ContributionsController : Controller
    {
        // GET: Contributions
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        // GET: Contributions/Dashboard
        public ActionResult Dashboard()
        {
            DashboardModelVM dashboardInfo = new DashboardModelVM();
            using(Db db = new Db())
            {
                // Fetch the user Id
                string username = User.Identity.Name;
                CustomerDTO customerDto = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch User Account Balance
                AccountContributionsDTO account = db.AccountContributions.Where(x => x.CustomerId == customerDto.Id).SingleOrDefault();
                if (account != null)
                    dashboardInfo.Account = new AccountContributionsVM(account);

                // fetch User investment Account
                CustomerInvestmentDTO investment = db.CustomerInvestments.Where(x => x.CustomerId == customerDto.Id).SingleOrDefault();
                dashboardInfo.InvestmentAccount = new CustomerInvestmentVM(investment);

                // Fetch Schedule for the next contribution
                ContributionScheduleDTO schedule = db.ContributionSchedules.Where(x => x.Paid == false && x.CustomerId == customerDto.Id).SingleOrDefault();
                if (schedule != null)
                    dashboardInfo.ContributionSchedule = new ContributionScheduleVM(schedule);

                // Fetch Beneficiaries
                dashboardInfo.Beneficiaries = db.Beneficiaries.Where(x => x.CustomerId == customerDto.Id).ToArray().Select(x => new BeneficiariesVM(x)).ToList();

            }
            return View(dashboardInfo);
        }

        // GET: Contributions/new-contribution
        [ActionName("new-contribution")]
        public ActionResult NewContribution()
        {
            // fetch username
            string username = User.Identity.Name;

            // Declare ContributonVM
            InitiatedContributionsVM model = new InitiatedContributionsVM();

            using (Db db = new Db())
            {
                // get the user from the database
                CustomerDTO dto = db.Customers.Where(x => x.Username == username).SingleOrDefault();
                CustomerContributionSettingsDTO settingsDTO =
                    db.CustomersCSettings.Where(x => x.CustomerId == dto.Id).SingleOrDefault();
                
                // get the selected contribution mode and deduction mode
                model.ContributionMode = settingsDTO.ContributionMode;
                model.DeductionMode = settingsDTO.DeductionMode;



                // Check if there is an ongoing contribution
                if(db.InitiatedContributions.Any(x => x.CustomerId == dto.Id && x.Finished == false))
                {
                    model.StartNew = false;
                }
                else
                {
                    model.StartNew = true;
                }

                // Check if there is at least on beneficiaries
                if(db.Beneficiaries.Any(x => x.CustomerId == dto.Id))
                {
                    model.Beneficiary = true;
                }
                else
                {
                    model.Beneficiary = false;
                }
                
            }
            string[] contributionMode = new string[] { "Daily", "Weekly", "Monthly", "Quarterly" };
            model.ContributionModeSelect = new SelectList(contributionMode.ToList());

            string[] deductionMode = new string[] { "One off", "Recurrent" };
            model.DeductionModeSelect = new SelectList(deductionMode.ToList());
            return View("NewContribution", model);
        }

        // POST: contributions/NewContribution
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("new-contribution")]
        public ActionResult NewContribution(InitiatedContributionsVM model)
        {
            string[] contributionMode = new string[] { "Daily", "Weekly", "Monthly", "Quarterly" };
            model.ContributionModeSelect = new SelectList(contributionMode.ToList());

            string[] deductionMode = new string[] { "One off", "Recurrent" };
            model.DeductionModeSelect = new SelectList(deductionMode.ToList());

            if (!ModelState.IsValid)
            {
                return View("NewContribution", model);
            }


            // declare the customerId
            int customerId;

            using (Db db = new Db())
            {
                // Fetch customer Id from the database
                string username = User.Identity.Name;
                CustomerDTO customerDTO = db.Customers.Where(x => x.Username == username).SingleOrDefault();
                customerId = customerDTO.Id;

                // converting the contribution mode to days
                if(model.ContributionMode == "Daily")
                {
                    model.ContributionDaysInterval = 1;
                }
                else if (model.ContributionMode == "Weekly")
                {
                    model.ContributionDaysInterval = 7;
                }
                else if(model.ContributionMode == "Monthly")
                {
                    model.ContributionDaysInterval = 30;
                }
                else
                {
                    model.ContributionDaysInterval = 120;
                }
                // Storing new contribution to Initiated contribution
                InitiatedContributionsDTO initiatedDTO = new InitiatedContributionsDTO();
                initiatedDTO.CustomerId = customerId;
                initiatedDTO.ContributionMode = model.ContributionMode;
                initiatedDTO.DeductionMode = model.DeductionMode;
                initiatedDTO.ContributionAmount = model.ContributionAmount;
                initiatedDTO.ContributionDaysInterval = model.ContributionDaysInterval;
                initiatedDTO.Finished = false;
                initiatedDTO.TotalAmount = 0;
                initiatedDTO.StartDate = DateTime.Now;
                initiatedDTO.Slug = "contribution-" + DateTime.Now.ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-");
                db.InitiatedContributions.Add(initiatedDTO);
                db.SaveChanges();
                int ContributionId = initiatedDTO.Id;

                // Pay if it is recurrent
                if(model.DeductionMode == "Recurrent")
                {
                    // Then pay to the admin account
                    AccountBalanceDTO balance = db.AccountBalance.Find(1);
                    balance.AccountBalance += initiatedDTO.ContributionAmount;
                    // Pay to your account
                    AccountContributionsDTO account = db.AccountContributions.Where(x => x.CustomerId == customerId).SingleOrDefault();
                    account.ContributionsAccount += initiatedDTO.ContributionAmount;
                    initiatedDTO.TotalAmount += initiatedDTO.ContributionAmount;
                    db.SaveChanges(); 

                    // Store payment info
                    ContributionProcessDTO process = new ContributionProcessDTO();
                    process.InitiatedContributionsId = ContributionId;
                    process.CustomerId = customerId;
                    process.AmountPaid = initiatedDTO.ContributionAmount;
                    process.TimeOfContribution = DateTime.Now;
                    process.ContributionType = initiatedDTO.ContributionMode;
                    process.DeductionMode = initiatedDTO.DeductionMode;
                    process.Invested = false;
                    db.ContributionProcess.Add(process);
                    db.SaveChanges();

                    

                    // Add Next Payment Schedule
                    ContributionScheduleDTO schedule = new ContributionScheduleDTO();
                    schedule.CustomerId = customerId;
                    schedule.InitiatedContributionId = ContributionId;
                    schedule.DateOfNextContribution = DateTime.Now.AddDays(model.ContributionDaysInterval);
                    schedule.Paid = false;
                    schedule.Cancel = false;
                    schedule.Amount = model.ContributionAmount;
                    db.ContributionSchedules.Add(schedule);
                    db.SaveChanges();
                    TempData["SM"] = "Contribution Scheme Created and your first contribution was successful.";
                }
                else
                {
                    // Add Next Payment Schedule for One off
                    ContributionScheduleDTO schedule = new ContributionScheduleDTO();
                    schedule.CustomerId = customerId;
                    schedule.InitiatedContributionId = ContributionId;
                    schedule.DateOfNextContribution = DateTime.Now;
                    schedule.Paid = false;
                    schedule.Cancel = false;
                    schedule.Amount = model.ContributionAmount;
                    db.ContributionSchedules.Add(schedule);
                    db.SaveChanges();

                    TempData["SM"] = "Contribution Scheme Created successfully. You can now start your contributions.";
                }

            }

            return RedirectToAction("ongoing-contributions");
        }

        //GET: Contributions/contribution-history
        [ActionName("ongoing-contributions")]
        public ActionResult OngoingContributions(int? page)
        {
            var pageNumber = page ?? 1;

            // Declare Ongoing Contribution VM
            OngoingContributionVM ongoingVM = new OngoingContributionVM();
            InitiatedContributionsDTO initiated;

            string username = User.Identity.Name;
            using(Db db = new Db())
            {
                CustomerDTO customer = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                initiated = db.InitiatedContributions.Where(x => x.CustomerId == customer.Id && x.Finished == false).SingleOrDefault();
                if(initiated != null)
                    ongoingVM.InitiatedContributionVM = new InitiatedContributionsVM(initiated);

                AccountContributionsDTO account = db.AccountContributions.Where(x => x.CustomerId == customer.Id).SingleOrDefault();
                ongoingVM.AccountVM = new AccountContributionsVM(account);

                if (initiated != null)
                    ongoingVM.ProcessVMs = db.ContributionProcess.ToArray().Where(x => x.CustomerId == customer.Id && x.InitiatedContributionsId == initiated.Id).OrderByDescending(x => x.TimeOfContribution).Select(x => new ContributionProcessVM(x)).ToList();

                ContributionScheduleDTO schedule = db.ContributionSchedules.Where(x => x.CustomerId == customer.Id && x.Paid == false && x.Cancel ==false).SingleOrDefault();
                if(schedule != null)
                    ongoingVM.ContributionScheduleVM = new ContributionScheduleVM(schedule);
            }

            if(initiated != null)
            {
                // Set pagination
                var onePageOfProcesses = ongoingVM.ProcessVMs.ToPagedList(pageNumber, 20);
                ViewBag.OnePageOfProcesses = onePageOfProcesses;

            }
            return View("OngoingContributions", ongoingVM);
        }

        //POST: Contributions/ContributeNow
        [HttpPost]
        public string ContributeNow(int id)
        {
            using(Db db = new Db())
            {
                // Get the schedule
                ContributionScheduleDTO scheduleDto = db.ContributionSchedules.Find(id);
                if(scheduleDto == null)
                {
                    TempData["SM"] = "Payment schedule not found.";
                    return "error";
                }

                // check if the date is ok
                if(scheduleDto.DateOfNextContribution > DateTime.Now)
                {
                    TempData["SM"] = "It's not yet time to contribute.";
                    return "error";
                }

                // check if the user has paid before
                if(scheduleDto.Paid == true)
                {
                    TempData["SM"] = "You have already contributed.";
                    return "error";
                }

                // fetch the user
                string username = User.Identity.Name;
                CustomerDTO customer = db.Customers.Where(x => x.Username == username).SingleOrDefault();
                int customerId = customer.Id;

                // fetch the initiated dto
                InitiatedContributionsDTO initiatedDTO = db.InitiatedContributions.Where(x => x.Id == scheduleDto.InitiatedContributionId).SingleOrDefault();
                int ContributionId = initiatedDTO.Id;

                // Then pay to the admin account
                AccountBalanceDTO balance = db.AccountBalance.Find(1);
                balance.AccountBalance += initiatedDTO.ContributionAmount;
                // Then make payment to your account
                AccountContributionsDTO account = db.AccountContributions.Where(x => x.CustomerId == customerId).SingleOrDefault();
                account.ContributionsAccount += initiatedDTO.ContributionAmount;
                initiatedDTO.TotalAmount += initiatedDTO.ContributionAmount;
                db.SaveChanges();

                // Store payment info
                ContributionProcessDTO process = new ContributionProcessDTO();
                process.InitiatedContributionsId = ContributionId;
                process.CustomerId = customerId;
                process.AmountPaid = initiatedDTO.ContributionAmount;
                process.TimeOfContribution = DateTime.Now;
                process.ContributionType = initiatedDTO.ContributionMode;
                process.DeductionMode = initiatedDTO.DeductionMode;
                process.Invested = false;
                db.ContributionProcess.Add(process);
                // Make your payment true on your schedule
                scheduleDto.Paid = true;
                db.SaveChanges();


                // Add Next Payment Schedule
                ContributionScheduleDTO schedule = new ContributionScheduleDTO();
                schedule.CustomerId = customerId;
                schedule.InitiatedContributionId = ContributionId;
                schedule.DateOfNextContribution = DateTime.Now.AddDays(initiatedDTO.ContributionDaysInterval);
                schedule.Paid = false;
                schedule.Cancel = false;
                schedule.Amount = initiatedDTO.ContributionAmount;
                db.ContributionSchedules.Add(schedule);
                db.SaveChanges();
                
            }
            TempData["SM"] = "You have successfully contributed for the day.";
            // return string
            return "success";
        }

        //POST: Contributions/CancelContribution
        [HttpPost]
        public void CancelContribution(int id)
        {
            using (Db db = new Db())
            {
                // check for the schedule and remove it
                ContributionScheduleDTO schedule = db.ContributionSchedules.Where(x => x.InitiatedContributionId == id && x.Cancel == false && x.Paid == false).SingleOrDefault();
                if(schedule != null)
                {
                    db.ContributionSchedules.Remove(schedule);
                    db.SaveChanges();
                }

                // change the initiated schedule to finished
                InitiatedContributionsDTO initiated = db.InitiatedContributions.Find(id);
                initiated.Finished = true;
                initiated.EndDate = DateTime.Now;
                db.SaveChanges();
            }

            // Set tempdata
            TempData["SM"] = "Your ongoing contributions has been stopped.";
        }

        public void Recurrent()
        {
            using (Db db = new Db())
            {
                List<ContributionScheduleDTO> schedules = db.ContributionSchedules.Where(x => x.Paid == false && x.Cancel == false).ToList();
                foreach (var item in schedules)
                {
                    // Check if it is recurrent
                    if(item.InitiatedContribution.DeductionMode == "Recurrent")
                    {
                        // Check if the contributions is due
                        if(item.DateOfNextContribution.Date == DateTime.Now.Date)
                        {
                            // Fetch the Customer
                            CustomerDTO customer = db.Customers.Where(x => x.Id == item.CustomerId).SingleOrDefault();
                            int customerId = customer.Id;

                            // Fetch the initiated contributions DTO
                            InitiatedContributionsDTO initiatedDTO = db.InitiatedContributions.Where(x => x.Id == item.InitiatedContributionId).SingleOrDefault();
                            int ContributionId = initiatedDTO.Id;

                            // Then pay to the admin account
                            AccountBalanceDTO balance = db.AccountBalance.Find(1);
                            balance.AccountBalance += initiatedDTO.ContributionAmount;
                            // Pay to your account
                            AccountContributionsDTO account = db.AccountContributions.Where(x => x.CustomerId == customerId).SingleOrDefault();
                            account.ContributionsAccount += initiatedDTO.ContributionAmount;
                            initiatedDTO.TotalAmount += initiatedDTO.ContributionAmount;
                            db.SaveChanges();

                            // Store payment info
                            ContributionProcessDTO process = new ContributionProcessDTO();
                            process.InitiatedContributionsId = ContributionId;
                            process.CustomerId = customerId;
                            process.AmountPaid = initiatedDTO.ContributionAmount;
                            process.TimeOfContribution = DateTime.Now;
                            process.ContributionType = initiatedDTO.ContributionMode;
                            process.DeductionMode = initiatedDTO.DeductionMode;
                            process.Invested = false;
                            db.ContributionProcess.Add(process);
                            item.Paid = true;
                            db.SaveChanges();

                            // Add Next Payment Schedule
                            ContributionScheduleDTO schedule = new ContributionScheduleDTO();
                            schedule.CustomerId = customerId;
                            schedule.InitiatedContributionId = ContributionId;
                            schedule.DateOfNextContribution = DateTime.Now.AddDays(initiatedDTO.ContributionDaysInterval);
                            schedule.Paid = false;
                            schedule.Cancel = false;
                            schedule.Amount = initiatedDTO.ContributionAmount;
                            db.ContributionSchedules.Add(schedule);
                            db.SaveChanges();

                        }

                    }
                }
            }
        }

        // GET: Contributions/terminated-contributions
        [ActionName("terminated-contributions")]
        public ActionResult TerminatedContributions(int? page)
        {
            var pageNumber = page ?? 1;

            // Declare a list of terminated contributions
            List<InitiatedContributionsVM> terminatedContributions;

            using(Db db = new Db())
            {
                //fetch the user
                string username = User.Identity.Name;
                CustomerDTO customer = db.Customers.Where(x => x.Username == username).SingleOrDefault();
                int customerId = customer.Id;

                terminatedContributions = db.InitiatedContributions.Where(x => x.CustomerId == customerId && x.Finished == true).OrderByDescending(x => x.EndDate).ToArray().Select(x => new InitiatedContributionsVM(x)).ToList();

            }
            ViewBag.PageNumber = pageNumber;

            // Set pagination
            var onePageOfContributions = terminatedContributions.ToPagedList(pageNumber, 10);
            ViewBag.OnePageOfContributions = onePageOfContributions;

            return View("TerminatedContributions", terminatedContributions);

        }


        // GET: Contributions/view-terminated-contribution/slug
        [ActionName("view-terminated-contribution")]
        public ActionResult ViewTerminatedContribution(string slug, int? page)
        {
            var pageNumber = page ?? 1;

            // Declare your View
            ViewTerminatedContributionVM model = new ViewTerminatedContributionVM();
            using(Db db = new Db())
            {
                // fetch the initiated contribution
                InitiatedContributionsDTO contributionsDto = db.InitiatedContributions.Where(x => x.Slug == slug).SingleOrDefault();
                model.InitiatedContribution = new InitiatedContributionsVM(contributionsDto);

                // fetch the contribution processes
                model.Processes = db.ContributionProcess.Where(x => x.InitiatedContributionsId == contributionsDto.Id).ToArray().Select(x => new ContributionProcessVM(x)).ToList();

            }
            ViewBag.PageNumber = pageNumber;

            // Set pagination
            var onePageOfProcesses = model.Processes.ToPagedList(pageNumber, 10);
            ViewBag.OnePageOfProcesses = onePageOfProcesses;

            return View("ViewTerminatedContribution", model);
        }


        // GET: Contributions/withdraw-cash
        [ActionName("withdraw-cash")]
        public ActionResult WithdrawCash()
        {
            // Declare you model
            CashWithdrawVM model = new CashWithdrawVM();

            using(Db db = new Db())
            {
                // fetch the user
                string username = User.Identity.Name;
                CustomerDTO customer = db.Customers.Where(x => x.Username == username).SingleOrDefault();
                int customerId = customer.Id;

                // fetch the customer account balance
                AccountContributionsDTO accountBalance = db.AccountContributions.Where(x => x.CustomerId == customerId).SingleOrDefault();
                model.AccountBalance = accountBalance.ContributionsAccount;

                // fetch the customer profit balance
                CustomerInvestmentDTO customerInvestment = db.CustomerInvestments.Where(x => x.CustomerId == customerId).SingleOrDefault();
                model.ProfitBalance = customerInvestment.AvailableProfit;

                // fetch previous request 
                WithdrawalRequestDTO request = db.WithdrawalRequests.Where(x => x.CustomerId == customerId && x.RespondBy == null).SingleOrDefault();
                if (request != null)
                    model.withdrawalRequest = true;
                else
                    model.withdrawalRequest = false;
            }

            return View("WithdrawCash", model);
        }

        // POST: Contributions/withdraw-cash
        [ActionName("withdraw-cash")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WithdrawCash(CashWithdrawVM model)
        {
            using (Db db = new Db())
            {
                // fetch the user
                string username = User.Identity.Name;
                CustomerDTO customer = db.Customers.Where(x => x.Username == username).SingleOrDefault();
                int customerId = customer.Id;

                // fetch the customer account balance
                AccountContributionsDTO accountBalance = db.AccountContributions.Where(x => x.CustomerId == customerId).SingleOrDefault();
                model.AccountBalance = accountBalance.ContributionsAccount;

                // fetch the customer profit balance
                CustomerInvestmentDTO customerInvestment = db.CustomerInvestments.Where(x => x.CustomerId == customerId).SingleOrDefault();
                model.ProfitBalance = customerInvestment.AvailableProfit;


                // Checking if the form submitted is valid
                if (!ModelState.IsValid)
                {
                    return View("WithdrawCash",model);
                }

                // checking if there is no money
                if(model.AmountToWithdraw < 1)
                {
                    ModelState.AddModelError("", "Sorry, you have not specified any amount to withdraw.");
                    return View("WithdrawCash", model);
                }

                // checking if the user is not request more than he has
                if(model.AmountToWithdraw > (model.AccountBalance + model.ProfitBalance))
                {
                    ModelState.AddModelError("", "Sorry, Insufficient fund.");
                    return View("WithdrawCash",model);
                }

                // Make request
                WithdrawalRequestDTO newRequest = new WithdrawalRequestDTO();
                newRequest.CustomerId = customerId;
                newRequest.Amount = model.AmountToWithdraw;
                newRequest.DateSent = DateTime.Now;
                newRequest.AdminStatus = false;
                newRequest.Slug = "request-" + customer.Username + "-" + DateTime.Now.ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-");
                newRequest.AccountBalance = model.AccountBalance;
                newRequest.AvailableProfit = model.ProfitBalance;
                db.WithdrawalRequests.Add(newRequest);
                db.SaveChanges();
            }

            TempData["SM"] = "Request Sent Successfully. You will be replied in a few moment.";

            return RedirectToAction("withdraw-cash");
        }

    }
}