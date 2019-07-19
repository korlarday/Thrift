using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thrift.Models.Data;
using Thrift.Models.ViewModel.Account;
using Thrift.Models.ViewModel.Contributions;
using PagedList;

namespace Thrift.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    public class TransactionsController : Controller
    {
        // GET: Admin/Transactions
        public ActionResult Personal()
        {
            // Declare list of InitiatedContributions VM
            List<InitiatedContributionsVM> Contributions;

            using(Db db = new Db())
            {
                Contributions = db.InitiatedContributions.ToArray().Select(x => new InitiatedContributionsVM(x)).ToList();

            }
            return View(Contributions);
        }

        // GET: Admin/Transactions
        [ActionName("credit-transactions")]
        public ActionResult CreditTransactions(int? page)
        {
            var pageNumber = page ?? 1;
            List<ContributionProcessVM> processes;
            List<ContributionProcessVM> filteredProcesses = new List<ContributionProcessVM>();

            using (Db db = new Db())
            {
                processes = db.ContributionProcess.ToArray().OrderByDescending(x=> x.TimeOfContribution).Select(x => new ContributionProcessVM(x)).ToList();
                if (User.IsInRole("All"))
                {
                    // Set pagination
                    var oneOfProcesses = processes.ToPagedList(pageNumber, 20);
                    ViewBag.OnePageOfProcesses = oneOfProcesses;
                    return View("CreditTransactions", processes);
                }

                // fetch the admin
                string username = User.Identity.Name;
                CustomerDTO admin = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the users he is managing
                List<int> users = db.CustomerAllocations.Where(x => x.AdminId == admin.Id).Select(x => x.CustomerId).ToList();

                foreach (var item in processes)
                {
                    if (users.Contains(item.CustomerId))
                    {
                        filteredProcesses.Add(item);
                    }
                }
            }

            // Set pagination
            var onePageOfProcesses = filteredProcesses.ToPagedList(pageNumber, 20);
            ViewBag.OnePageOfProcesses = onePageOfProcesses;
            return View("CreditTransactions", filteredProcesses);
        }

        // GET: Admin/Transactions/debit-transactions 
        [ActionName("debit-transactions")]
        public ActionResult DebitTransactions(int? page)
        {
            var pageNumber = page ?? 1;
            List<WithdrawalRequestVM> withdrawals;
            List<WithdrawalRequestVM> filteredwithdrawals = new List<WithdrawalRequestVM>();

            using (Db db = new Db())
            {
                withdrawals = db.WithdrawalRequests.Where(x => x.Approve == true).ToArray().OrderByDescending(x => x.DateSent).Select(x => new WithdrawalRequestVM(x)).ToList();
                
                if (User.IsInRole("All"))
                {
                    // Set pagination
                    var oneOfProcesses = withdrawals.ToPagedList(pageNumber, 20);
                    ViewBag.OnePageOfProcesses = oneOfProcesses;
                    return View("DebitTransactions", withdrawals);
                }

                // fetch the admin
                string username = User.Identity.Name;
                CustomerDTO admin = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the users he is managing
                List<int> users = db.CustomerAllocations.Where(x => x.AdminId == admin.Id).Select(x => x.CustomerId).ToList();

                foreach (var item in withdrawals)
                {
                    if (users.Contains(item.CustomerId))
                    {
                        filteredwithdrawals.Add(item);
                    }
                }
            }

            // Set pagination
            var onePageOfProcesses = filteredwithdrawals.ToPagedList(pageNumber, 20);
            ViewBag.OnePageOfProcesses = onePageOfProcesses;
            return View("DebitTransactions", filteredwithdrawals);
        }

        // GET: Admin/Transactions

        // GET: Admin/Transactions/Requests 
        public ActionResult Requests()
        {
            List<WithdrawalRequestVM> requests;
            List<WithdrawalRequestVM> filteredRequests = new List<WithdrawalRequestVM>();
            using (Db db = new Db())
            {
                requests = db.WithdrawalRequests.ToArray().OrderByDescending(x => x.DateSent).Select(x => new WithdrawalRequestVM(x)).ToList();
                if (User.IsInRole("All"))
                {
                    return View(requests);
                }

                // fetch the admin
                string username = User.Identity.Name;
                CustomerDTO admin = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the users he is managing
                List<int> customers = db.CustomerAllocations.Where(x => x.AdminId == admin.Id).Select(x => x.CustomerId).ToList();

                foreach (var item in requests)
                {
                    if (customers.Contains(item.CustomerId))
                    {
                        filteredRequests.Add(item);
                    }
                }
            }

            return View(filteredRequests);
        }

        // GET: Admin/Transactions/view-request/slug
        [ActionName("view-request")]
        public ActionResult ViewRequest(string slug)
        {
            WithdrawalRequestVM request;
            using (Db db = new Db())
            {
                WithdrawalRequestDTO requestDTO = db.WithdrawalRequests.Where(x => x.Slug == slug).SingleOrDefault();

                // fetch the admin
                string username = User.Identity.Name;
                CustomerDTO admin = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the users he is managing
                List<int> customers = db.CustomerAllocations.Where(x => x.AdminId == admin.Id).Select(x => x.CustomerId).ToList();

                if (!User.IsInRole("All"))
                {
                    if (!customers.Contains(requestDTO.CustomerId))
                    {
                        return Redirect("/Admin/Dashboard");
                    }
                }

                if (requestDTO.AdminStatus == false)
                {
                    requestDTO.AdminStatus = true;
                    db.SaveChanges();
                }
                request = new WithdrawalRequestVM(requestDTO);
            }

            return View("ViewRequest",request);
        }

        [HttpPost]
        public ActionResult RejectRequest(string reason, string userSlug)
        {
            if(reason == "")
            {
                TempData["Error"] = "You Must state a reason for rejection.";
                return RedirectToAction("view-request", new { slug = userSlug });
            }

            using (Db db = new Db())
            {
                // fetch the user
                string username = User.Identity.Name;
                CustomerDTO customer = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the withdraw request and modify
                WithdrawalRequestDTO requestDto = db.WithdrawalRequests.Where(x => x.Slug == userSlug).SingleOrDefault();
                requestDto.RespondBy = customer.Id;
                requestDto.Approve = false;
                db.SaveChanges();

                // create a message for the customer
                CustomerMessagesDTO message = new CustomerMessagesDTO();
                message.Title = "Response to your Request";
                message.Body = reason;
                message.RequestReply = true;
                message.Approve = false;
                message.TimeSent = DateTime.Now;
                message.CustomerId = requestDto.CustomerId;
                message.Status = false;
                message.Slug = "message-" + DateTime.Now.ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-");
                db.Messages.Add(message);
                db.SaveChanges();
            }
            // Set temp data
            TempData["SM"] = "Withdrawal Request has been rejected.";
            return RedirectToAction("view-request", new { slug = userSlug });
        }


        public ActionResult AcceptRequest(string slug)
        {
            using(Db db = new Db())
            {
                // fetch the user
                string username = User.Identity.Name;
                CustomerDTO customerAdmin = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the request
                WithdrawalRequestDTO request = db.WithdrawalRequests.Where(x => x.Slug == slug).SingleOrDefault();

                // fetch the contributions account
                AccountContributionsDTO contributionAcct = db.AccountContributions.Where(x => x.CustomerId == request.CustomerId).SingleOrDefault();

                // fetch Total Account balance
                AccountBalanceDTO accountBalance = db.AccountBalance.Find(1);

                // fetch Total Investment Balance
                InvestmentBalanceDTO investmentBalance = db.InvestmentBalance.Find(1);

                // fetch the profit balance
                CustomerInvestmentDTO investment = db.CustomerInvestments.Where(x => x.CustomerId == request.CustomerId).SingleOrDefault();
                decimal availableProfit = investment.AvailableProfit;
                decimal totalMoney = request.Customer.Account.ContributionsAccount + availableProfit;
                decimal requestMoney = request.Amount;

                // THIS CONDITION MAY STILL NEED TO BE REVIEWED
                // check if the customer withdraws everything
                if (totalMoney == request.Amount)
                {
                    // if yes stop the current contribution
                    InitiatedContributionsDTO initiatedDto = db.InitiatedContributions.Where(x => x.CustomerId == request.CustomerId && x.Finished == false).SingleOrDefault();
                    if(initiatedDto != null)
                    {
                        initiatedDto.Finished = true;
                        db.SaveChanges();
                    }
                    ContributionScheduleDTO schedule = db.ContributionSchedules.Where(x => x.CustomerId == request.CustomerId && x.Paid == false && x.Cancel == false).SingleOrDefault();
                    if(schedule != null)
                    {
                        db.ContributionSchedules.Remove(schedule);
                        db.SaveChanges();
                    }

                }

                // First Lets take money from the Profit
                if(availableProfit >= request.Amount)
                {
                    requestMoney = investment.AvailableProfit - request.Amount;
                    investment.AvailableProfit -= request.Amount;
                    investment.ProfitWithdrawn += request.Amount;
                    db.SaveChanges();
                }
                else
                {
                    // if the request amount if greater than the available profit
                    requestMoney = request.Amount - investment.AvailableProfit;
                    investment.AvailableProfit = 0;
                    db.SaveChanges();

                    // We move on to Uninvestment Cash
                    // to get the uninvested cash we will subtract total balance from invested cash
                    AccountContributionsDTO customerAcct = db.AccountContributions.Where(x => x.CustomerId == request.CustomerId).SingleOrDefault();
                    decimal unInvestedCash = customerAcct.ContributionsAccount - investment.InvestmentBalance;
                    if(unInvestedCash > 1)
                    {
                        // if the uninvested cash is greater than the remining request money
                        if(unInvestedCash > requestMoney)
                        {
                            // remove the money from conribution i.e uninvested cash
                            contributionAcct.ContributionsAccount -= requestMoney;
                            contributionAcct.WithdrawAccount += requestMoney;
                            db.SaveChanges();

                            // remove money from the total contribution balance
                            accountBalance.AccountBalance -= requestMoney;
                            accountBalance.WithdrawalBalance += requestMoney;
                            db.SaveChanges();
                        }
                        else
                        {
                            // if the remining request money is greater than the uninvested cash
                            // subtract uninvestedcash from the contributions account
                            contributionAcct.ContributionsAccount -= unInvestedCash;
                            contributionAcct.WithdrawAccount += unInvestedCash;
                            db.SaveChanges();

                            // subtract from the main account balance
                            accountBalance.AccountBalance -= unInvestedCash;
                            accountBalance.WithdrawalBalance += unInvestedCash;
                            db.SaveChanges();
                            
                            // subtract uninvestedcash from the remainaing request money
                            requestMoney -= unInvestedCash;

                            // Subtract the cash from invested money
                            investment.InvestmentBalance -= requestMoney;
                            db.SaveChanges();
                            // subtact from the main investment balance
                            investmentBalance.AccountBalance -= requestMoney;
                            db.SaveChanges();

                            // subtract from contributions account
                            contributionAcct.ContributionsAccount -= requestMoney;
                            contributionAcct.WithdrawAccount += requestMoney;
                            db.SaveChanges();

                            // subtract from the main Contribution Account Balance
                            accountBalance.AccountBalance -= requestMoney;
                            accountBalance.WithdrawalBalance += requestMoney;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        // Subtract the cash from invested money
                        investment.InvestmentBalance -= requestMoney;
                        db.SaveChanges();
                        // subtact from the main investment balance
                        investmentBalance.AccountBalance -= requestMoney;
                        db.SaveChanges();

                        // subtract from contributions account
                        contributionAcct.ContributionsAccount -= requestMoney;
                        contributionAcct.WithdrawAccount += requestMoney;
                        db.SaveChanges();

                        // subtract from the main Contribution Account Balance
                        accountBalance.AccountBalance -= requestMoney;
                        accountBalance.WithdrawalBalance += requestMoney;
                        db.SaveChanges();
                    }
                }


                // update the request
                request.Approve = true;
                request.RespondBy = customerAdmin.Id;
                db.SaveChanges();

                // Create a message for the user
                CustomerMessagesDTO message = new CustomerMessagesDTO();
                message.Title = "Response to your request";
                message.Body = "Dear Customer, Your request was accepted successfully. " +
                                "Your current contribution balance is " + contributionAcct.ContributionsAccount + "." +
                                "Your available profit is " + investment.AvailableProfit + ".";
                message.RequestReply = true;
                message.Status = false;
                message.TimeSent = DateTime.Now;
                message.Approve = true;
                message.CustomerId = request.CustomerId;
                message.Slug = "message-" + DateTime.Now.ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-");
                db.Messages.Add(message);
                db.SaveChanges();

            }
            // redirect back to the page
            // Set temp data
            TempData["SM"] = "Withdrawal Request has been accepted.";
            return RedirectToAction("view-request", new { slug = slug });
        }
    }
}