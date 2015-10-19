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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace FileSync.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersController : Controller
    {

        public ActionResult Index()
        {
            return View(FileSyncDal.Instance.GetAllUsers());
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,Place,Email")] FileSyncUser user)
        {
            if (ModelState.IsValid)
            {
                var existUser = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(user.Id);
                if (existUser == null)
                    return HttpNotFound();

                existUser.UserName = user.UserName;
                existUser.Place = user.Place;
                existUser.Email = user.Email;

                HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().Update(existUser);
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(id);
            if (user == null)
                return HttpNotFound();

            HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().Delete(user);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // add dispose here
            }
            base.Dispose(disposing);
        }
    }
}
