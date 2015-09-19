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

namespace FileSync.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        // GET: Files
        public ActionResult Index()
        {
            var files = FileSyncDal.GetAllFiles(User.Identity);
            return View(files);
        }

        // GET: Files/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = FileSyncDal.GetFile(User.Identity, id.Value);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // GET: Files/Create
        public ActionResult Create()
        {
            var folders = FileSyncDal.GetAllFolders(User.Identity);
            ViewBag.ParentFolderId = new SelectList(folders, "Id", "Name");
            return View();
        }

        // POST: Files/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Path,ParentFolderId")] File file)
        {
            if (ModelState.IsValid)
            {
                FileSyncDal.AddFile(file);
                return RedirectToAction("Index");
            }

            var folders = FileSyncDal.GetAllFolders(User.Identity);
            ViewBag.ParentFolderId = new SelectList(folders, "Id", "Name", file.ParentFolderId);
            return View(file);
        }

        // GET: Files/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            File file = FileSyncDal.GetFile(User.Identity, id.Value);
            if (file == null)
            {
                return HttpNotFound();
            }
            var folders = FileSyncDal.GetAllFolders(User.Identity);
            ViewBag.ParentFolderId = new SelectList(folders, "Id", "Name", file.ParentFolderId);
            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Path,ParentFolderId")] File file)
        {
            if (ModelState.IsValid)
            {
                FileSyncDal.SaveEditFile(file);
                return RedirectToAction("Index");
            }
            var folders = FileSyncDal.GetAllFolders(User.Identity);
            ViewBag.ParentFolderId = new SelectList(folders, "Id", "Name", file.ParentFolderId);
            return View(file);
        }

        // GET: Files/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = FileSyncDal.GetFile(User.Identity, id.Value);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            File file = FileSyncDal.GetFile(User.Identity, id);
            FileSyncDal.RemoveFile(file);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // do dispose
            }
            base.Dispose(disposing);
        }
    }
}
