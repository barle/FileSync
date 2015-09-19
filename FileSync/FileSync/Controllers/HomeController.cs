using FileSync.DAL;
using FileSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FileSync.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index(int? parentFolderId)
        {
            if (parentFolderId == null)
            {
                var rootFolders = FileSyncDal.GetRootFolders(User.Identity);
                var viewModel = new HomeViewModel(){Folders = rootFolders};
                return View(viewModel);
            }

            var parentFolder = FileSyncDal.GetFolder(User.Identity, parentFolderId.Value);
            if (parentFolder == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var homeViewModel = new HomeViewModel()
            {
                Files = parentFolder.Files,
                Folders = parentFolder.SubFolders,
                ParentFolder = parentFolder
            };
            return View(homeViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}