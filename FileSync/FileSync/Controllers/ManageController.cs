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
            return RedirectToAction("Index", "ManageUsers");
        }

        public ActionResult ManageGroups()
        {
            return RedirectToAction("Index", "ManageGroups");
        }

        public ActionResult ManageFolders()
        {
            return RedirectToAction("Index", "ManageFolders");
        }
    }
}