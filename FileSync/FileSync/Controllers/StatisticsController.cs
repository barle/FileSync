using FileSync.DAL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileSync.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetFilesPerDays(int days)
        {
            var daysToFilesCount = FileSyncDal.GetFilesPerDays(days);
            return Json(daysToFilesCount, JsonRequestBehavior.AllowGet);
        }
    }
}