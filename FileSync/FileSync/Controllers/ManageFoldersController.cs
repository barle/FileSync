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

namespace FileSync.Controllers
{
    [Authorize]
    public class ManageFoldersController : Controller
    {
        // GET: Folders
        public ActionResult Index()
        {
            var folders = FileSyncDal.GetAllFolders(User.Identity);
            return View(folders.ToList());
        }

        // GET: Folders/Details/5
        [ItemAuthorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Folder folder = FileSyncDal.GetFolder(User.Identity, id.Value);
            if (folder == null)
            {
                return HttpNotFound();
            }
            return View(folder);
        }

        // GET: Folders/Create
        public ActionResult Create()
        {
            var folders = FileSyncDal.GetAllFolders(User.Identity);
            ViewBag.ParentFolderId = new SelectList(folders, "Id", "Name");
            return View();
        }

        // POST: Folders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ParentFolderId")] Folder folder)
        {
            if (ModelState.IsValid)
            {
                FileSyncDal.AddFolder(folder);
                return RedirectToAction("Index");
            }

            var folders = FileSyncDal.GetAllFolders(User.Identity);
            ViewBag.ParentFolderId = new SelectList(folders, "Id", "Name", folder.ParentFolderId);
            return View(folder);
        }

        // GET: Folders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Folder folder = FileSyncDal.GetFolder(User.Identity, id.Value);
            if (folder == null)
            {
                return HttpNotFound();
            }

            var folders = FileSyncDal.GetAllFolders(User.Identity);
            ViewBag.ParentFolderId = new SelectList(folders, "Id", "Name", folder.ParentFolderId);
            return View(folder);
        }

        // POST: Folders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ParentFolderId")] Folder folder)
        {
            if (ModelState.IsValid)
            {
                FileSyncDal.SaveEditFolder(folder);
                return RedirectToAction("Index");
            }

            var folders = FileSyncDal.GetAllFolders(User.Identity);
            ViewBag.ParentFolderId = new SelectList(folders, "Id", "Name", folder.ParentFolderId);
            return View(folder);
        }

        // GET: Folders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Folder folder = FileSyncDal.GetFolder(User.Identity, id.Value);
            if (folder == null)
            {
                return HttpNotFound();
            }
            return View(folder);
        }

        // POST: Folders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Folder folder = FileSyncDal.GetFolder(User.Identity, id);
            FileSyncDal.RemoveFolder(folder);
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
