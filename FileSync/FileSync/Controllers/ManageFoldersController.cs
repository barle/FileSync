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
    [Authorize]
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
