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

        public ActionResult GetFilesPerDays()
        {
            var results = new List<object>();
            //foreach
            results.Add(new { x = "1", y = "10" });
            results.Add(new { x = "2", y = "30" });
            results.Add(new { x = "3", y = "25" });
            results.Add(new { x = "4", y = "35" });
            results.Add(new { x = "5", y = "3" });


            return Json(results.ToArray(), JsonRequestBehavior.AllowGet);
        }
    }
}