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
        [ItemAuthorize("folder",true)]
        public ActionResult Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                var rootFolders = FileSyncDal.Instance.GetRootFolders(User.Identity);
                var viewModel = new HomeViewModel(){Folders = rootFolders};
                return View(viewModel);
            }

            var parentFolder = FileSyncDal.Instance.GetFolder(User.Identity, id);
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
            var file = FileSyncDal.Instance.GetFile(User.Identity, id);
            if(file == null)
            {
                return HttpNotFound();
            }
            return File(file.Path, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
        }

        public ActionResult About()
        {
            return View();
        }

        [ItemAuthorize("file")]
        public ActionResult ShowVideoFile(string id)
        {
            if(id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var videoFile = FileSyncDal.Instance.GetFile(User.Identity, id);
            if(videoFile == null)
                return HttpNotFound();

            var videoType = videoFile.Extension == ".wmv" ? "x-ms-wmv" : videoFile.Extension.ToLower().Substring(1);
            var model = new ShowVideoViewModel()
            {
                VideoPath = "../DownloadFile/" + videoFile.Id,
                VideoName = videoFile.Name,
                VideoType = videoType,
                ReturnUrl = "Home/Index/" + videoFile.ParentFolderId
            };

            return View(model);
        }
    }
}