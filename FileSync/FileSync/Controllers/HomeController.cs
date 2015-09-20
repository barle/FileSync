using FileSync.Authorization;
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
        public ActionResult Index(string parentFolderId)
        {
            if (string.IsNullOrWhiteSpace(parentFolderId))
            {
                var rootFolders = FileSyncDal.GetRootFolders(User.Identity);
                var viewModel = new HomeViewModel(){Folders = rootFolders};
                return View(viewModel);
            }

            var parentFolder = FileSyncDal.GetFolder(User.Identity, parentFolderId);
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

        [ItemAuthorize("file")]
        public ActionResult DownloadFile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var file = FileSyncDal.GetFile(User.Identity, id);
            if(file == null)
            {
                return HttpNotFound();
            }
            return File(file.Path, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
        }

    }
}