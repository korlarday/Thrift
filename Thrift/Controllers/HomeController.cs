using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Thrift.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Features()
        {
            return View();
        }

        public ActionResult Benefits()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult SubMenuPartial()
        {
            return PartialView();
        }

    }
}