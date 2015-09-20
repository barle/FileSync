using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FileSync.Models;
using FileSync.DAL;
using FileSync.Authorization;
using FileSync.Mapper;

namespace FileSync.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageFoldersController : Controller
    {
        // GET: Folders
        public ActionResult Index()
        {
            var folders = FileSyncDal.GetRootFolders(User.Identity);
            return View(folders.ToList());
        }

        // GET: Folders/Details/5
        [ItemAuthorize("folder")]
        public ActionResult Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Folder folder = FileSyncDal.GetFolder(User.Identity, id);
            if (folder == null)
            {
                return HttpNotFound();
            }
            return View(folder);
        }

        // GET: Folders/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Path")] Folder folder)
        {
            if (ModelState.IsValid)
            {
                var folderMapper = new FolderMapper(folder);
                folderMapper.Map();
                return RedirectToAction("Index");
            }

            return View(folder);
        }

        // GET: Folders/Edit/5
        [ItemAuthorize("folder")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Folder folder = FileSyncDal.GetFolder(User.Identity, id);
            if (folder == null)
            {
                return HttpNotFound();
            }
            return View(folder);
        }

        // POST: Folders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ItemAuthorize("folder")]
        public ActionResult Edit([Bind(Include = "Id,Name")] Folder folder)
        {
            if (ModelState.IsValid)
            {
                var existFolder = FileSyncDal.GetFolder(User.Identity, folder.Id);
                if (existFolder == null)
                    return HttpNotFound();
                existFolder.Name = folder.Name;
                FileSyncDal.SaveEditFolder(existFolder);
                return RedirectToAction("Index");
            }
            return View(folder);
        }

        // GET: Folders/Delete/5
        [ItemAuthorize("folder")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Folder folder = FileSyncDal.GetFolder(User.Identity, id);
            if (folder == null)
            {
                return HttpNotFound();
            }
            return View(folder);
        }

        // POST: Folders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ItemAuthorize("folder")]
        public ActionResult DeleteConfirmed(string id)
        {
            FileSyncDal.RemoveFolder(id);
            return RedirectToAction("Index");
        }

        public ActionResult ManageAuthorizations(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Folder folder = FileSyncDal.GetFolder(User.Identity, id);
            if (folder == null)
            {
                return HttpNotFound();
            }
            return View(folder);
        }

        public ActionResult SearchUsersToAuthorize(string folderId, string userName, string groupName, int? groupsCount)
        {
            if (folderId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Folder folder = FileSyncDal.GetFolder(User.Identity, folderId);
            if (folder == null)
                return HttpNotFound();

            var searchParams = new FileSync.Models.UserSearchParams(userName, groupName, groupsCount);
            var users = FileSyncDal.GetUsersBySearch(searchParams);
            var model = new LinkUserViewModel()
            {
                CallbackAction = "AuthorizeUserToFolder",
                CallbackController = "ManageFolders",
                Users = users.Where(u => !folder.AuthorizedUsers.Any(fu => fu.Id == u.Id)),// users that are not already authorized to this folder
                ParentId = folderId
            };
            ViewBag.PageIcon = "lock";
            ViewBag.PageIconDescription = "Authorize Users to Folder";
            ViewBag.Title = "Link Users";
            ViewBag.ReturnUrl = "ManageFolders/ManageAuthorizations/" + folderId;

            return View("../Links/LinkUsersView", model);
        }

        [HttpPost]
        public ActionResult AuthorizeUserToFolder(string parentId, string userId)
        {
            if (parentId == null || userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FileSyncDal.AddUserToFolder(parentId, userId);
            return RedirectToAction("SearchUsersToAuthorize", new { folderId = parentId });
        }

        [HttpPost]
        public ActionResult UnAuthorizeUserFromFolder(string folderId, string userId)
        {
            if (folderId == null || userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FileSyncDal.RemoveUserFromFolder(folderId, userId);
            return RedirectToAction("ManageAuthorizations", new { id = folderId });
        }

        public ActionResult SearchGroupsToAuthorize(string folderId, string groupName, string userName, int? membersCount)
        {
            if (folderId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Folder folder = FileSyncDal.GetFolder(User.Identity, folderId);
            if (folder == null)
                return HttpNotFound();

            var searchParams = new FileSync.Models.GroupSearchParams(groupName, userName, membersCount);
            var groups = FileSyncDal.GetGroupsBySearch(searchParams);
            var model = new LinkGroupViewModel()
            {
                CallbackAction = "AuthorizeGroupToFolder",
                CallbackController = "ManageFolders",
                Groups = groups.Where(g => !folder.AuthorizedGroups.Any(fg => fg.Id == g.Id)),// groups that are not already authorized to this folder
                ParentId = folderId
            };
            ViewBag.PageIcon = "lock";
            ViewBag.PageIconDescription = "Authorize Groups to Folder";
            ViewBag.Title = "Link Groups";
            ViewBag.ReturnUrl = "ManageFolders/ManageAuthorizations/" + folderId;

            return View("../Links/LinkGroupsView", model);
        }

        [HttpPost]
        public ActionResult AuthorizeGroupToFolder(string parentId, string groupId)
        {
            if (parentId == null || groupId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FileSyncDal.AddGroupToFolder(parentId, groupId);
            return RedirectToAction("SearchGroupsToAuthorize", new { folderId = parentId });
        }

        [HttpPost]
        public ActionResult UnAuthorizeGroupFromFolder(string folderId, string groupId)
        {
            if (folderId == null || groupId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FileSyncDal.RemoveGroupFromFolder(folderId, groupId);
            return RedirectToAction("ManageAuthorizations", new { id = folderId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // do dispose here
            }
            base.Dispose(disposing);
        }
    }
}
