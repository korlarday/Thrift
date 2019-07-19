using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thrift.Areas.Admin.Models.Investment;
using Thrift.Models.Data;
using Thrift.Models.ViewModel.Investment;
using PagedList;

namespace Thrift.Areas.Admin.Controllers
{
    [Authorize(Roles = "Investment")]
    public class InvestmentsController : Controller
    {
        // GET: Admin/Investments
        public ActionResult Index()
        {
            return RedirectToAction("invest-cash-view");
        }

        // GET: Admin/Investments/invest-cash-view
        [ActionName("invest-cash-view")]
        public ActionResult InvestCashView()
        {
            AmountToInvestVM model = new AmountToInvestVM();
            decimal TotalAvailableCash = 0;

            using(Db db = new Db())
            {
                List<AccountContributionsDTO> accounts = db.AccountContributions.Where(x => !(x.ContributionsAccount < 1)).ToList();
                foreach (var item in accounts)
                {
                    int cusId = item.CustomerId;

                    // fetch your invested cash
                    CustomerInvestmentDTO investment = db.CustomerInvestments.Where(x => x.CustomerId == cusId).SingleOrDefault();
                    decimal availableCash = item.ContributionsAccount - investment.InvestmentBalance;
                    TotalAvailableCash += availableCash;

                    //if (CheckNewPayments > 0)
                    //{
                    //    List<ContributionProcessDTO> processes = db.ContributionProcess.Where(x => x.Invested == true && x.CustomerId == item.CustomerId).ToList();
                    //    if(processes != null)
                    //    {
                    //        foreach (var process in processes)
                    //        {
                    //            ProcessCash += process.AmountPaid;
                    //        }
                    //        AvailableCash = (item.ContributionsAccount + (decimal)item.WithdrawAccount) - ProcessCash;
                    //        TotalAvailableCash += AvailableCash;
                    //    }
                    //}

                }

                InvestmentBalanceDTO balance = db.InvestmentBalance.Find(1);
                model.InvestmentBalance = balance.AccountBalance;
            }
            model.AmountToInvest = TotalAvailableCash;
            return View("InvestCashView", model);
        }

        // GET: Admin/Investments/invest-cash
        [ActionName("invest-cash")]
        public ActionResult InvestCash()
        {

            using (Db db = new Db())
            {
                // Fetch all the accounts
                List<AccountContributionsDTO> accounts = db.AccountContributions.Where(x => !(x.ContributionsAccount < 1)).ToList();


                // Loop throught the accounts
                foreach (var item in accounts)
                {
                    // declare the available
                    decimal AvailableCash = 0;

                    int cusId = item.CustomerId;

                    // fetch your invested cash
                    CustomerInvestmentDTO investment = db.CustomerInvestments.Where(x => x.CustomerId == cusId).SingleOrDefault();
                    decimal availableCash = item.ContributionsAccount - investment.InvestmentBalance;
                    
                    // Add the cash to the customer investment balance
                    investment.InvestmentBalance += AvailableCash;
                    db.SaveChanges();

                    // Add the cash to the Main Investment Balance
                    InvestmentBalanceDTO balance = db.InvestmentBalance.Find(1);
                    balance.AccountBalance += AvailableCash;
                    db.SaveChanges();
                }
            }
            TempData["SM"] = "Cash invested successfully";
            return RedirectToAction("invest-cash-view");
        }

        [ActionName("select-available-cash")]
        public ActionResult SelectAvailableCash()
        {
            // Declare your model
            SelectAvailableCashVM model = new SelectAvailableCashVM();
            model.Customers = new List<CustomerAndAmount>();
            
            using(Db db = new Db())
            {
                // fetch all accounts that is not empty
                List<AccountContributionsDTO> accounts = db.AccountContributions.Where(x => !(x.ContributionsAccount < 1)).ToList();
                foreach (var item in accounts)
                {
                    int cusId = item.CustomerId;
                    decimal AvailableCash = 0;

                    CustomerInvestmentDTO investment = db.CustomerInvestments.Where(x => x.CustomerId == cusId).SingleOrDefault();

                    
                    // Calculating the available cash
                    AvailableCash = item.ContributionsAccount - investment.InvestmentBalance;

                    if(AvailableCash > 0)
                    {
                        // Add the cash to the customer investment balance
                        CustomerAndAmount customerandAmt = new CustomerAndAmount();

                        /////// populate the customer and amount
                        CustomerDTO customer = db.Customers.Find(item.CustomerId);
                        customerandAmt.Username = customer.Username;
                        customerandAmt.Id = item.CustomerId;
                        customerandAmt.AmountAvailableToInvest = AvailableCash;

                        model.Customers.Add(customerandAmt);

                    }
                }

                // Fetch current contribution balance
                model.CurrentInvestBalance = db.InvestmentBalance.Find(1).AccountBalance;
            }
            return View("SelectAvailableCash", model);
        }

        // ajax request
        [HttpPost]
        public string InvestSelectedCash(List<int> customers)
        {
            using(Db db = new Db())
            {
                foreach (var item in customers)
                {
                    int cusId = item;
                    decimal AvailableCash = 0;

                    // Fetch the contribution account
                    AccountContributionsDTO ContributionsAccount = db.AccountContributions.Where(x => x.CustomerId == item).SingleOrDefault();

                    // Investment Cash
                    CustomerInvestmentDTO investment = db.CustomerInvestments.Where(x => x.CustomerId == cusId).SingleOrDefault();

                    // Calculating the available cash
                    AvailableCash = ContributionsAccount.ContributionsAccount - investment.InvestmentBalance;

                    // Add the cash to the customer investment balance
                    investment.InvestmentBalance += AvailableCash;
                    db.SaveChanges();

                    // Add the cash to the Main Investment Balance
                    InvestmentBalanceDTO balance = db.InvestmentBalance.Find(1);
                    balance.AccountBalance += AvailableCash;
                    db.SaveChanges();
                }
            }
            TempData["SM"] = "Selected cash invested successfully";
            return "success";
        }

        // Enter Profit
        // GET: Admin/Investments/enter-profit
        [ActionName("enter-profit")]
        public ActionResult EnterProfit()
        {
            // Declare your model
            EnterProfitVM model = new EnterProfitVM();
            return View("EnterProfit", model);
        }

        // Submit Profit
        // GET: Admin/Investments/enter-profit
        [HttpPost]
        [ActionName("enter-profit")]
        [ValidateAntiForgeryToken]
        public ActionResult EnterProfit(EnterProfitVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("EnterProfit", model);
            }

            decimal cash = model.Profit;
            using(Db db = new Db())
            {
                // generate a unique slug
                // profit-16-07-2019-20-51-00-20000
                string slug = "profit-" +
                    DateTime.Now.ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-") +
                    "-" + cash;
                // Store the Profit
                ProfitDTO profit = new ProfitDTO();
                profit.DateOfPayment = DateTime.Now;
                profit.Amount = cash;
                profit.Slug = slug;
                db.Profits.Add(profit);
                db.SaveChanges();


                // 30% of profit goes to the management;
                decimal managementCut = cash * 30 / 100;
                cash -= managementCut;

                // make the management payment
                InvestmentBalanceDTO investmentBalance = db.InvestmentBalance.Find(1);
                investmentBalance.ProfitBalance += managementCut;
                db.SaveChanges();

                // store the management payment details
                ProfitProcessIBDTO processIB = new ProfitProcessIBDTO();
                processIB.DateOfPayment = DateTime.Now;
                processIB.Amount = managementCut;
                processIB.ProfitId = profit.Id;
                db.ProfitProcessIBs.Add(processIB);
                db.SaveChanges();

                // Store each of the customers cash Percentage
                List<CustomerInvestmentDTO> customerInvestments = db.CustomerInvestments.Where(x => !(x.InvestmentBalance < 1)).ToList();

                foreach (var item in customerInvestments)
                {
                    // calculate the percentage
                    decimal percentage = 100 * (item.InvestmentBalance / investmentBalance.AccountBalance);
                    // make the customer profit payment
                    CustomerInvestmentDTO investment = db.CustomerInvestments.Where(x => x.CustomerId == item.CustomerId).SingleOrDefault();
                    decimal customerProfit = (percentage / 100) * cash;
                    investment.AvailableProfit += customerProfit;
                    db.SaveChanges();

                    // store the profit payment process
                    ProfitProcessDTO profitProcess = new ProfitProcessDTO();
                    profitProcess.CustomerId = item.CustomerId;
                    profitProcess.Date = DateTime.Now;
                    profitProcess.Amount = customerProfit;
                    profitProcess.Percentage = percentage;
                    profitProcess.ProfitId = profit.Id;
                    db.ProfitProcesses.Add(profitProcess);
                    db.SaveChanges();
                }

            }

            TempData["SM"] = "Profit Payments was made successfully.";
            return RedirectToAction("enter-profit");
        }

        // GET: Admin/Investments/previous-profits
        [ActionName("previous-profits")]
        public ActionResult PreviousProfit(int? page)
        {
            List<ProfitVM> model;
            var pageNumber = page ?? 1;
            using (Db db = new Db())
            {
                model = db.Profits.OrderByDescending(x => x.DateOfPayment).ToArray().Select(x => new ProfitVM(x)).ToList();

            }
            // Set pagination
            var onePageOfProcesses = model.ToPagedList(pageNumber, 10);
            ViewBag.OnePageOfProcesses = onePageOfProcesses;
            return View("PreviousProfit", model);
        }

        [ActionName("profit-distribution")]
        public ActionResult ProfitDistribution(int? page, string slug)
        {
            var pageNumber = page ?? 1;

            ProfitDistributionVM model = new ProfitDistributionVM();

            using(Db db = new Db())
            {
                // Fetch the profit 
                ProfitDTO profit = db.Profits.Where(x => x.Slug == slug).SingleOrDefault();
                model.ProfitAmount = profit.Amount;

                model.Proccess = db.ProfitProcesses.Where(x => x.ProfitId == profit.Id).OrderByDescending(x => x.Amount).ToArray().Select(x => new ProfitProcessVM(x)).ToList();

                ProfitProcessIBDTO processIBDTO = db.ProfitProcessIBs.Where(x => x.ProfitId == profit.Id).SingleOrDefault();
                model.ProfitProcessIB = new ProfitProcessIBVM(processIBDTO);
            }

            // Set pagination
            var onePageOfProcesses = model.Proccess.ToPagedList(pageNumber, 20);
            ViewBag.OnePageOfProcesses = onePageOfProcesses;
            return View("ProfitDistribution", model);
        }
    }
}