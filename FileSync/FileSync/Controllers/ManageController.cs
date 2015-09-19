using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileSync.Controllers
{
    [Authorize(Roles="Admin")]
    public class ManageController : Controller
    {
        // GET: Manage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageUsers()
        {
            return View();
        }

        public ActionResult ManageGroups()
        {
            return View();
        }

        public ActionResult ManageFolders()
        {
            return RedirectToAction("Index", "ManageFolders");
        }
    }
}