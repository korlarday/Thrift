using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Thrift.Models.Data;
using Thrift.Models.ViewModel;
using Thrift.Models.ViewModel.Account;
using Thrift.Models.ViewModel.Contributions;

namespace Thrift.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }
        
        // GET: Account/Login
        public ActionResult Login()
        {
            // Confirm user is not logged in

            string username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
                return Redirect("/Home");

            // Return View
            return View();
        }

        // POST: Account/Login
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if the user is valid
            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Customers.Any(x => x.Username == model.Username && x.Password == model.Password))
                {
                    isValid = true;
                }
            }

            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }
            else
            {
                TempData["SM"] = "Welcome back " + model.Username + "!";
                FormsAuthentication.SetAuthCookie(model.Username, false);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, false));
            }
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            // Confirm user is not logged in

            string username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
                return Redirect("/Home");

            RegistrationVM model = new RegistrationVM();
            string[] contributionMode = new string[] {"Daily","Weekly", "Monthly", "Quarterly"};
            model.ContributionModeSelect = new SelectList(contributionMode.ToList());

            string[] deductionMode = new string[] {"One off", "Recurrent"};
            model.DeductionModeSelect = new SelectList(deductionMode.ToList());

            model.DOB = new DateTime(2000, 01, 01); 
            return View(model);
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegistrationVM model)
        {
            // populate the select lists
            string[] contributionMode = new string[] { "Daily", "Weekly", "Monthly", "Quarterly" };
            model.ContributionModeSelect = new SelectList(contributionMode.ToList());

            string[] deductionMode = new string[] { "One off", "Recurrent" };
            model.DeductionModeSelect = new SelectList(deductionMode.ToList());

            // check if model state is valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string username = model.Username.Trim().Replace(" ", "-");
            // Declare user id
            int id;
            using (Db db = new Db())
            {
                // check if username is unique
                if (db.Customers.Any(x => x.Username == username))
                {
                    ModelState.AddModelError("", "The username already exists.");
                    return View(model);
                }

                // Check if passwords match
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords do not match");
                    return View(model);
                }

                #region Save Customer
                CustomerDTO customerDTO = new CustomerDTO();
                customerDTO.FirstName = model.FirstName;
                customerDTO.LastName = model.LastName;
                customerDTO.OtherName = model.OtherName;
                customerDTO.Address = model.Address;
                customerDTO.StateOfOrigin = model.StateOfOrigin;
                customerDTO.LGA = model.LGA;
                customerDTO.PhoneNumber = model.PhoneNumber;
                customerDTO.Town = model.Town;
                customerDTO.DOB = model.DOB;
                customerDTO.Username = model.Username;
                customerDTO.Password = model.Password;
                customerDTO.Email = model.Email;

                db.Customers.Add(customerDTO);
                db.SaveChanges();
                id = customerDTO.Id;
                #endregion
                #region Save Next of Kin
                NextOfKinDTO kin = new NextOfKinDTO();
                kin.CustomerId = id;
                kin.Name = model.NextOfKinName;
                kin.Email = model.NextOfKinEmail;
                kin.PhoneNumber = model.NextOfKinPhoneNumber;
                db.NextOfKins.Add(kin);
                db.SaveChanges();
                #endregion
                #region save customer contribution setting 
                CustomerContributionSettingsDTO customerCSDTO = new CustomerContributionSettingsDTO();
                customerCSDTO.CustomerId = id;
                customerCSDTO.ContributionMode = model.ContributionMode;
                customerCSDTO.DeductionMode = model.DeductionMode;
                db.CustomersCSettings.Add(customerCSDTO);
                db.SaveChanges();
                #endregion
                #region save account details

                AccountInfoDTO accountDTO = new AccountInfoDTO();
                accountDTO.BankName = model.BankName;
                accountDTO.CustomerId = id;
                accountDTO.AccountNumber = model.AccountNumber;
                accountDTO.AccountName = model.AccountName;
                db.AccountInfos.Add(accountDTO);
                db.SaveChanges();

                #endregion
                #region save investment account

                CustomerInvestmentDTO investmentAccount = new CustomerInvestmentDTO();
                investmentAccount.CustomerId = id;
                investmentAccount.InvestmentBalance = 0;
                investmentAccount.AvailableProfit = 0;
                db.CustomerInvestments.Add(investmentAccount);
                db.SaveChanges();

                #endregion
                #region save customer role

                CustomerRoles customerRoles = new CustomerRoles();
                customerRoles.CustomerId = id;
                customerRoles.RoleId = 1;
                db.CustomerRoles.Add(customerRoles);
                db.SaveChanges();

                #endregion
                #region Save Account balance
                AccountContributionsDTO account = new AccountContributionsDTO();
                account.ContributionsAccount = 0;
                account.CustomerId = id;
                account.WithdrawAccount = 0;
                db.AccountContributions.Add(account);
                db.SaveChanges();
                //save Accountid to customer table
                customerDTO.AccountId = account.Id;
                db.SaveChanges();
                #endregion
            }

            #region Upload Passport

            // Create necessary directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Passports");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Passports\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Passports\\" + id.ToString() + "\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);


            // Check if a file was uploaded
            if (model.Passport != null && model.Passport.ContentLength > 0)
            {
                // Get file extension
                string ext = model.Passport.ContentType.ToLower();

                // Verify extension
                if (ext == "image/jpg" ||
                    ext == "image/jpeg" ||
                    ext == "image/pjpeg" ||
                    ext == "image/gif" ||
                    ext == "image/x-png" ||
                    ext == "image/png")
                {

                    //  Init image name
                    string imageName = model.Passport.FileName;

                    // Save image name to DTO
                    using (Db db = new Db())
                    {
                        CustomerDTO dto = db.Customers.Find(id);
                        dto.PassportName = imageName;

                        db.SaveChanges();
                    }

                    // Set original and thumb image path
                    var path = string.Format("{0}\\{1}", pathString2, imageName);
                    var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                    //  Save original
                    model.Passport.SaveAs(path);

                    // Create and save thumb
                    WebImage img = new WebImage(model.Passport.InputStream);
                    img.Resize(150, 150);
                    img.Save(path2);
                }
            }
            #endregion
            #region Upload Signature

            // Create necessary directories
            var originalDirectory1 = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathStrng1 = Path.Combine(originalDirectory.ToString(), "Signatures");
            var pathStrng2 = Path.Combine(originalDirectory.ToString(), "Signatures\\" + id.ToString());
            var pathStrng3 = Path.Combine(originalDirectory.ToString(), "Signatures\\" + id.ToString() + "\\Thumbs");

            if (!Directory.Exists(pathStrng1))
                Directory.CreateDirectory(pathStrng1);

            if (!Directory.Exists(pathStrng2))
                Directory.CreateDirectory(pathStrng2);

            if (!Directory.Exists(pathStrng3))
                Directory.CreateDirectory(pathStrng3);


            // Check if a file was uploaded
            if (model.Signature != null && model.Signature.ContentLength > 0)
            {
                // Get file extension
                string ext = model.Signature.ContentType.ToLower();

                // Verify extension
                if (ext == "image/jpg" ||
                    ext == "image/jpeg" ||
                    ext == "image/pjpeg" ||
                    ext == "image/gif" ||
                    ext == "image/x-png" ||
                    ext == "image/png")
                {

                    //  Init image name
                    string signatureName = model.Signature.FileName;

                    // Save image name to DTO
                    using (Db db = new Db())
                    {
                        CustomerDTO dto = db.Customers.Find(id);
                        dto.SignatureName = signatureName;

                        db.SaveChanges();
                    }

                    // Set original and thumb image path
                    var path = string.Format("{0}\\{1}", pathString2, signatureName);
                    var path2 = string.Format("{0}\\{1}", pathString3, signatureName);

                    //  Save original
                    model.Signature.SaveAs(path);

                    // Create and save thumb
                    WebImage img = new WebImage(model.Signature.InputStream);
                    img.Resize(150, 150);
                    img.Save(path2);
                }
            }
            #endregion

            TempData["SM"] = "You are registered successfully on KoboKobo.";

            FormsAuthentication.SetAuthCookie(model.Username, false);
            return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, false));
            
        }

        [Authorize]
        [ActionName("view-profile")]
        public ActionResult ViewProfile()
        {
            RegistrationVM profile = new RegistrationVM();
            string username = User.Identity.Name;
            using(Db db = new Db())
            {
                CustomerDTO customer = db.Customers.Where(x => x.Username == username).SingleOrDefault();
                profile.FirstName = customer.FirstName;
                profile.LastName = customer.LastName;
                profile.OtherName = customer.OtherName;
                profile.DOB = customer.DOB;
                profile.StateOfOrigin = customer.StateOfOrigin;
                profile.LGA = customer.LGA;
                profile.Town = customer.Town;
                profile.PhoneNumber = customer.PhoneNumber;
                profile.Email = customer.Email;
                profile.PassportName = customer.PassportName;
                profile.Id = customer.Id;
                profile.Username = customer.Username;
                profile.Address = customer.Address;

                NextOfKinDTO kinDto = db.NextOfKins.Where(x => x.CustomerId == customer.Id).SingleOrDefault();
                profile.NextOfKinName = kinDto.Name;
                profile.NextOfKinEmail = kinDto.Email;
                profile.NextOfKinPhoneNumber = kinDto.PhoneNumber;

                AccountInfoDTO account = db.AccountInfos.Where(x => x.CustomerId == customer.Id).SingleOrDefault();
                profile.BankName = account.BankName;
                profile.AccountName = account.AccountName;
                profile.AccountNumber = account.AccountNumber;

                profile.Beneficiaries = db.Beneficiaries.Where(x => x.CustomerId == customer.Id).ToArray().Select(x => new BeneficiariesVM(x)).ToList();

            }
            return View("ViewProfile", profile);
        }

        [Authorize]
        [ActionName("add-beneficiary")]
        public ActionResult AddBeneficiary()
        {
            return View("AddBeneficiary");
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddBeneficiary(BeneficiariesVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddBeneficiary", model);
            }

            using (Db db = new Db())
            {
                // Fetch the user
                string username = User.Identity.Name;
                CustomerDTO customer = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // populate the beneficiary
                BeneficiariesDTO beneficiaries = new BeneficiariesDTO();
                beneficiaries.Name = model.Name;
                beneficiaries.Email = model.Email;
                beneficiaries.PhoneNumber = model.PhoneNumber;
                beneficiaries.CustomerId = customer.Id;
                db.Beneficiaries.Add(beneficiaries);
                db.SaveChanges();

                TempData["SM"] = "Beneficiary added successfully";
            }
            return RedirectToAction("view-profile");
        }

        // GET: /Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/Home");
        }

        [Authorize]
        public ActionResult UserNavPartial()
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
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Username = dto.Username
                };
            }

            // Return partial view with model
            return PartialView(model);
        }

        [Authorize]
        public ActionResult UsernamePartial()
        {
            // Get name
            string username = User.Identity.Name;

            // Declare model
            UserNavPartialVM model;

            using (Db db = new Db())
            {
                // Get the user
                CustomerDTO dto = db.Customers.SingleOrDefault(x => x.Username == username);

                // count new messages
                int MessageNum = db.Messages.Where(x => x.CustomerId == dto.Id && x.Status == false).Count();
                
                // Build the model
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Username = dto.Username,
                    NewMessages = MessageNum
                };
            }

            // Return partial view with model
            return PartialView(model);
        }

        [Authorize]
        [ActionName("view-messages")]
        public ActionResult ViewMessages()
        {
            // Declare list of withdrawal request
            List<CustomerMessagesVM> messages;

            using(Db db = new Db())
            {
                // fetch the user
                string username = User.Identity.Name;
                CustomerDTO customer = db.Customers.Where(x => x.Username == username).SingleOrDefault();

                // fetch the messages
                messages = db.Messages.Where(x => x.CustomerId == customer.Id).OrderByDescending(x => x.TimeSent).ToArray().Select(x => new CustomerMessagesVM(x)).ToList();

            }
            return View("ViewMessages", messages);
        }

        [Authorize]
        public JsonResult ViewMessage(string slug)
        {
            CustomerMessagesVM message;
            using(Db db = new Db())
            {
                // Fetch the message
                CustomerMessagesDTO messageDTO = db.Messages.Where(x => x.Slug == slug).SingleOrDefault();

                if (messageDTO.Status == false)
                {
                    messageDTO.Status = true;
                    db.SaveChanges();
                }
                message = new CustomerMessagesVM(messageDTO);
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchStates()
        {
            List<StateData> states = new List<StateData>();
            using (StreamReader sr = new StreamReader(Server.MapPath("~/Content/States.json")))
            {
                states = JsonConvert.DeserializeObject<List<StateData>>(sr.ReadToEnd());
            }
            return this.Json(states, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchLga(string state)
        {
            StateData detail = new StateData();

            using (StreamReader sr = new StreamReader(Server.MapPath("~/Content/States.json")))
            {
                List<StateData> states = new List<StateData>();
                states = JsonConvert.DeserializeObject<List<StateData>>(sr.ReadToEnd());

                foreach (var item in states)
                {
                    if(item.State == state)
                    {
                        detail = item;
                    }
                }
            }
            return this.Json(detail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchBanks()
        {
            List<BankData> banks = new List<BankData>();
            using (StreamReader sr = new StreamReader(Server.MapPath("~/Content/Banks.json")))
            {
                banks = JsonConvert.DeserializeObject<List<BankData>>(sr.ReadToEnd());
            }
            return this.Json(banks, JsonRequestBehavior.AllowGet);
        }

    }
}