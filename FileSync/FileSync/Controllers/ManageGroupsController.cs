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
    [Authorize]
    public class ManageGroupsController : Controller
    {

        // GET: Groups
        public ActionResult Index()
        {
            return View(FileSyncDal.GetAllGroups());
        }

        // GET: Groups/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = FileSyncDal.GetGroup(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        public ActionResult ManageGroupUsers(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = FileSyncDal.GetGroup(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        public ActionResult SearchUsersToAdd(string groupId, string userName, string groupName, int? groupsCount)
        {
            if (groupId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Group group = FileSyncDal.GetGroup(groupId);
            if (group == null)
                return HttpNotFound();

            var searchParams = new FileSync.Models.UserSearchParams(userName, groupName, groupsCount);
            var users = FileSyncDal.GetUsersBySearch(searchParams);
            var model = new LinkUserViewModel()
            {
                CallbackAction = "AddUserToGroup",
                CallbackController = "ManageGroups",
                Users = users.Where(u => !group.Users.Any(gu => gu.Id == u.Id)),// users that are not already in this group
                ParentId = groupId
            };
            ViewBag.PageIcon = "star";
            ViewBag.PageIconDescription = "Add Users to Group";
            ViewBag.Title = "Link Users";
            ViewBag.ReturnUrl = "ManageGroups/ManageGroupUsers/" + groupId;

            return View("../Links/LinkUsersView", model);
        }

        [HttpPost]
        public ActionResult AddUserToGroup(string parentId, string userId)
        {
            FileSyncDal.AddUserToGroup(parentId, userId);
            return RedirectToAction("SearchUsersToAdd", new { groupId = parentId });
        }

        [HttpPost]
        public ActionResult RemoveUserFromGroup(string groupId, string userId)
        {
            if (groupId == null || userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FileSyncDal.RemoveUserFromGroup(groupId, userId);
            return RedirectToAction("ManageGroupUsers", new { id = groupId });
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DisplayName")] Group group)
        {
            if (ModelState.IsValid)
            {
                FileSyncDal.AddGroup(group);
                return RedirectToAction("Index");
            }

            return View(group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = FileSyncDal.GetGroup(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DisplayName")] Group group)
        {
            if (ModelState.IsValid)
            {
                FileSyncDal.SaveEditGroup(group);
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = FileSyncDal.GetGroup(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            FileSyncDal.RemoveGroup(id);
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
