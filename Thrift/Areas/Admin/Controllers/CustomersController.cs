using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thrift.Areas.Admin.Models;
using Thrift.Models.Data;
using Thrift.Models.ViewModel.Account;
using Thrift.Models.ViewModel.Contributions;
using PagedList;
using Thrift.Models.ViewModel.Investment;

namespace Thrift.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    public class CustomersController : Controller
    {
        // GET: Admin/Customer
        public ActionResult Index()
        {
            List<CustomerVM> customers;
            List<CustomerVM> filteredCustomers = new List<CustomerVM>();
            using (Db db = new Db())
            {
                customers = db.Customers.ToArray().Select(x => new CustomerVM(x)).ToList();
                if (User.IsInRole("All"))
                {
                    return View(customers);
                }

                // fetch the admin
                string username = User.Identity.Name;
                CustomerDTO admin = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the users he is managing
                List<int> users = db.CustomerAllocations.Where(x => x.AdminId == admin.Id).Select(x => x.CustomerId).ToList();

                foreach (var item in customers)
                {
                    if (users.Contains(item.Id))
                    {
                        filteredCustomers.Add(item);
                    }
                }
            }
            return View(filteredCustomers);
        }


        [ActionName("view-profile")]
        public ActionResult ViewProfile(string slug)
        {
            // Declare the customer
            CustomerVM customer;

            // fetch the user from the database
            using(Db db = new Db())
            {
                CustomerDTO customerDto = db.Customers.Where(x => x.Username == slug).SingleOrDefault();

                customer = new CustomerVM(customerDto);

                // fetch the ongoing contributions
                ContributionScheduleDTO schedule = db.ContributionSchedules.Where(x => x.Paid == false && x.Cancel == false && x.CustomerId == customer.Id).SingleOrDefault();
                if (schedule != null)
                    customer.OngoingContributions = new ContributionScheduleVM(schedule);

                // fetch the terminated contibutions
                customer.TerminatedContributions = db.InitiatedContributions.Where(x => x.Finished == true && x.CustomerId == customer.Id).ToArray().Select(x => new InitiatedContributionsVM(x)).ToList();

                // fetch the account balance
                AccountContributionsDTO account = db.AccountContributions.Where(x => x.CustomerId == customer.Id).SingleOrDefault();
                customer.Account = new AccountContributionsVM(account);

                // fetch the investment balance
                CustomerInvestmentDTO investmentDto = db.CustomerInvestments.Where(x => x.CustomerId == customer.Id).SingleOrDefault();
                customer.Investment = new CustomerInvestmentVM(investmentDto);

                if (User.IsInRole("All"))
                {
                    return View("ViewProfile", customer);
                }

                // fetch the admin
                string username = User.Identity.Name;
                CustomerDTO admin = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the users he is managing
                List<int> users = db.CustomerAllocations.Where(x => x.AdminId == admin.Id).Select(x => x.CustomerId).ToList();

                if (!users.Contains(customer.Id))
                {
                    return Redirect("~/Admin/Dashboard");
                }


            }

            return View("ViewProfile", customer);
        } 

        [ActionName("view-terminated")]
        public ActionResult ViewTerminated(string slug, int? page)
        {
            var pageNumber = page ?? 1;

            // Declare your View
            ViewTerminatedContributionVM model = new ViewTerminatedContributionVM();
            using (Db db = new Db())
            {
                // fetch the initiated contribution
                InitiatedContributionsDTO contributionsDto = db.InitiatedContributions.Where(x => x.Slug == slug).SingleOrDefault();
                model.InitiatedContribution = new InitiatedContributionsVM(contributionsDto);

                // fetch the contribution processes
                model.Processes = db.ContributionProcess.Where(x => x.InitiatedContributionsId == contributionsDto.Id).ToArray().Select(x => new ContributionProcessVM(x)).ToList();

                // fetch the username
                CustomerDTO customer = db.Customers.Find(contributionsDto.CustomerId);
                model.Username = customer.Username;

                ViewBag.PageNumber = pageNumber;

                // Set pagination
                var onePageOfProcesses = model.Processes.ToPagedList(pageNumber, 10);
                ViewBag.OnePageOfProcesses = onePageOfProcesses;

                if (User.IsInRole("All"))
                {
                    return View("ViewTerminated", model);
                }

                // fetch the admin
                string username = User.Identity.Name;
                CustomerDTO admin = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the users he is managing
                List<int> users = db.CustomerAllocations.Where(x => x.AdminId == admin.Id).Select(x => x.CustomerId).ToList();

                if (!users.Contains(contributionsDto.CustomerId))
                {
                    return Redirect("~/Admin/Dashboard");
                }
            }

            return View("ViewTerminated", model);
        }

        [ActionName("ongoing-contributions")]
        public ActionResult OngoingContributions(string slug, int? page)
        {
            var pageNumber = page ?? 1;

            OngoingContributionVM contributionVM = new OngoingContributionVM();
            using(Db db = new Db())
            {
                //fetch the user
                CustomerDTO customerDto = db.Customers.Where(x => x.Username == slug).SingleOrDefault();
                int customerId = customerDto.Id;

                // put the username in the VM
                contributionVM.Username = customerDto.Username;

                // fetch the initiated ongoing contribution
                InitiatedContributionsDTO ongoingInitiated = db.InitiatedContributions.Where(x => x.Finished == false && x.CustomerId == customerId).SingleOrDefault();
                if (ongoingInitiated != null)
                    contributionVM.InitiatedContributionVM = new InitiatedContributionsVM(ongoingInitiated);

                // fetch the next schedule
                ContributionScheduleDTO schedule = db.ContributionSchedules.Where(x => x.CustomerId == customerId && x.Cancel == false && x.Paid == false).SingleOrDefault();
                if (schedule != null)
                    contributionVM.ContributionScheduleVM = new ContributionScheduleVM(schedule);

                // FETCH the account
                AccountContributionsDTO account = db.AccountContributions.Where(x => x.CustomerId == customerId).SingleOrDefault();
                contributionVM.AccountVM = new AccountContributionsVM(account);

                if (ongoingInitiated != null)
                    contributionVM.ProcessVMs = db.ContributionProcess.ToArray().Where(x => x.CustomerId == customerId && x.InitiatedContributionsId == ongoingInitiated.Id).OrderByDescending(x => x.TimeOfContribution).Select(x => new ContributionProcessVM(x)).ToList();

                if (ongoingInitiated != null)
                {
                    // Set pagination
                    var onePageOfProcesses = contributionVM.ProcessVMs.ToPagedList(pageNumber, 20);
                    ViewBag.OnePageOfProcesses = onePageOfProcesses;

                }

                if (User.IsInRole("All"))
                {
                    return View("OngoingContributions", contributionVM);
                }

                // fetch the admin
                string username = User.Identity.Name;
                CustomerDTO admin = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the users he is managing
                List<int> users = db.CustomerAllocations.Where(x => x.AdminId == admin.Id).Select(x => x.CustomerId).ToList();

                if (!users.Contains(customerId))
                {
                    return Redirect("~/Admin/Dashboard");
                }
            }
            return View("OngoingContributions", contributionVM);
        }

        [ActionName("customer-allocations")]
        [Authorize(Roles = "Allocation")]
        public ActionResult CustomerAllocations()
        {
            // Declare your model
            AllocationPageVM model = new AllocationPageVM();
            model.AdminAndCustomers = new List<AdminAndCustomersVM>();
            model.UnAllocatedCustomers = new List<CustomerVM>();
            using(Db db = new Db())
            {
                
                // fetch all admins
                List<CustomerRoles> AdminIds = db.CustomerRoles.Where(x => x.RoleId == 2).ToList();
                
                foreach (var item in AdminIds)
                {
                    // Declare admin and customers
                    AdminAndCustomersVM adminAndCustomer = new AdminAndCustomersVM();

                    // now fetch the allocated customers to the admin
                    List<CustomerAllocationVM> adminCustomers = db.CustomerAllocations.Where(x => x.AdminId == item.CustomerId).ToArray().Select(x => new CustomerAllocationVM(x)).ToList();
                    
                    // fetch the admin
                    CustomerDTO admin = db.Customers.Find(item.CustomerId);
                    CustomerVM adminVM = new CustomerVM(admin);
                    adminAndCustomer.Admin = adminVM;

                    adminAndCustomer.Customers = adminCustomers;
                    model.AdminAndCustomers.Add(adminAndCustomer);
                }

                // Fetch the unAllocated customers
                List<int> AllocatedCustomers = db.CustomerAllocations.ToArray().Select(x => x.CustomerId).ToList();

                // Fetch all customers
                List<CustomerVM> AllCustomers = db.Customers.ToArray().Select(x => new CustomerVM(x)).ToList();

                // filter the unallocated ones
                foreach (var item in AllCustomers)
                {
                    if (!AllocatedCustomers.Contains(item.Id))
                    {
                        model.UnAllocatedCustomers.Add(item);
                    }
                }
            }

            return View("CustomerAllocations", model);
        }

        [ActionName("view-allocations")]
        [Authorize(Roles = "Allocation")]
        public ActionResult ViewAllocations(string slug)
        {
            // Declare your model
            AdminAllocationVM model = new AdminAllocationVM();
            model.AdminAndCustomer = new AdminAndCustomersVM();
            model.UnAllocatedCustomers = new List<CustomerVM>();

            using(Db db = new Db())
            {
                // Fetch the administrator
                CustomerDTO customerDto = db.Customers.Where(x => x.Username == slug).SingleOrDefault();
                CustomerVM customerVm = new CustomerVM(customerDto);
                model.AdminAndCustomer.Admin = customerVm;

                // Fetch the allocated users
                model.AdminAndCustomer.Customers = db.CustomerAllocations.Where(x => x.AdminId == customerDto.Id).ToArray().Select(x => new CustomerAllocationVM(x)).ToList();

                // Fetch unAllocated Customers
                // Fetch the unAllocated customers
                List<int> AllocatedCustomers = db.CustomerAllocations.ToArray().Select(x => x.CustomerId).ToList();

                // Fetch all customers
                List<CustomerVM> AllCustomers = db.Customers.ToArray().Select(x => new CustomerVM(x)).ToList();

                // filter the unallocated ones
                foreach (var item in AllCustomers)
                {
                    if (!AllocatedCustomers.Contains(item.Id))
                    {
                        model.UnAllocatedCustomers.Add(item);
                    }
                }
            }
            return View("ViewAllocations", model);
        }

        [Authorize(Roles = "Allocation")]
        public string AssignCustomers(List<int> customers, int adminId)
        {
            using (Db db = new Db())
            {
                // store it shap shap in the customer allocation
                foreach (var item in customers)
                {
                    CustomerAllocationDTO allocation = new CustomerAllocationDTO();
                    allocation.AdminId = adminId;
                    allocation.CustomerId = item;
                    db.CustomerAllocations.Add(allocation);
                    db.SaveChanges();
                }
            }
            TempData["SM"] = "The selected customers have been added successfully to this admin.";
            return "success";
        }

        [Authorize(Roles = "Allocation")]
        public string RemoveCustomer(int cusId)
        {
            using (Db db = new Db())
            {
                CustomerAllocationDTO allocation = db.CustomerAllocations.Where(x => x.CustomerId == cusId).SingleOrDefault();
                db.CustomerAllocations.Remove(allocation);
                db.SaveChanges();
            }
            TempData["SM"] = "Customer was successfully removed from this admin.";
            return "success";
        }

        [ActionName("manage-users")]
        [Authorize(Roles = "Allocation")]
        public ActionResult ManageUsers()
        {
            // Declare your model
            ManageUsersVM model = new ManageUsersVM();
            model.Administrators = new List<CustomerVM>();
            model.Nonadministrators = new List<CustomerVM>();

            using(Db db = new Db())
            {
                // Fetch all the users
                List<CustomerVM> customers = db.Customers.ToArray().Select(x => new CustomerVM(x)).ToList();

                List<int> adminIds = db.CustomerRoles.Where(x => x.RoleId == 2).Select(x => x.CustomerId).ToList();

                foreach (var item in customers)
                {
                    if (adminIds.Contains(item.Id))
                    {
                        model.Administrators.Add(item);
                    }
                    else
                    {
                        model.Nonadministrators.Add(item);
                    }
                }
            }

            return View("ManageUsers",model);
        }

        [ActionName("view-previledges")]
        [Authorize(Roles = "Allocation")]
        public ActionResult ViewPreviledges(string slug)
        {
            // Declare your model
            AuthorizeVM model = new AuthorizeVM();
            
            using (Db db = new Db())
            {
                CustomerDTO customer = db.Customers.Where(x => x.Username == slug).SingleOrDefault();
                model.Customer = new CustomerVM(customer);

                // fetch all roles
                List<int> roles = db.CustomerRoles.Where(x => x.CustomerId == customer.Id).Select(x => x.RoleId).ToList();

                if (roles.Contains(2))
                {
                    model.Admin = true;
                }
                if (roles.Contains(3))
                {
                    model.Allocation = true;
                }
                if (roles.Contains(4))
                {
                    model.All = true;
                }
                if (roles.Contains(5))
                {
                    model.Investment = true;
                }
            }
            return View("ViewPreviledges", model);
        }

        [Authorize(Roles = "Allocation")]
        [HttpPost]
        public string SetPrevideges(SetAuthorization model)
        {
            using(Db db = new Db())
            {

                // fetch all roles
                List<int> roles = db.CustomerRoles.Where(x => x.CustomerId == model.Id).Select(x => x.RoleId).ToList();


                if (model.Admin)
                {
                    if (!roles.Contains(2))
                    {
                        CustomerRoles role = new CustomerRoles();
                        role.RoleId = 2;
                        role.CustomerId = model.Id;
                        db.CustomerRoles.Add(role);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (roles.Contains(2))
                    {
                        CustomerRoles role = db.CustomerRoles.Where(x => x.CustomerId == model.Id && x.RoleId == 2).SingleOrDefault();
                        db.CustomerRoles.Remove(role);
                        db.SaveChanges();
                    }
                }

                if (model.Allocation)
                {
                    if (!roles.Contains(3))
                    {
                        CustomerRoles role = new CustomerRoles();
                        role.RoleId = 3;
                        role.CustomerId = model.Id;
                        db.CustomerRoles.Add(role);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (roles.Contains(3))
                    {
                        CustomerRoles role = db.CustomerRoles.Where(x => x.CustomerId == model.Id && x.RoleId == 3).SingleOrDefault();
                        db.CustomerRoles.Remove(role);
                        db.SaveChanges();
                    }
                }

                if (model.All)
                {
                    if (!roles.Contains(4))
                    {
                        CustomerRoles role = new CustomerRoles();
                        role.RoleId = 4;
                        role.CustomerId = model.Id;
                        db.CustomerRoles.Add(role);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (roles.Contains(4))
                    {
                        CustomerRoles role = db.CustomerRoles.Where(x => x.CustomerId == model.Id && x.RoleId == 4).SingleOrDefault();
                        db.CustomerRoles.Remove(role);
                        db.SaveChanges();
                    }
                }

                if (model.Investment)
                {
                    if (!roles.Contains(5))
                    {
                        CustomerRoles role = new CustomerRoles();
                        role.RoleId = 5;
                        role.CustomerId = model.Id;
                        db.CustomerRoles.Add(role);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (roles.Contains(5))
                    {
                        CustomerRoles role = db.CustomerRoles.Where(x => x.CustomerId == model.Id && x.RoleId == 5).SingleOrDefault();
                        db.CustomerRoles.Remove(role);
                        db.SaveChanges();
                    }
                }
            }
            TempData["SM"] = "Settings was saved successfully";
            return "success";
        }

        [HttpPost]
        public string sendMessage(MsgVm model)
        {
            using (Db db = new Db())
            {
                CustomerMessagesDTO message = new CustomerMessagesDTO()
                {
                    Title = model.Title,
                    Body = model.Text,
                    RequestReply = false,
                    Status = false,
                    TimeSent = DateTime.Now,
                    Approve = false,
                    CustomerId = model.Id,
                    Slug = "message-" + DateTime.Now.ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-")
                    
                };
                db.Messages.Add(message);
                db.SaveChanges();
            }
            TempData["SM"] = "Message was sent successfully";
            return "success";
        }
    }
}