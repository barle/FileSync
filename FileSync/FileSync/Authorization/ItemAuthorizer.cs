using FileSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using FileSync.DAL;

namespace FileSync.Authorization
{
    public class ItemAuthorizer
    {
        private static object _lockObj = new object();
        private static ItemAuthorizer _instance;

        public static ItemAuthorizer Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                lock (_lockObj)
                {
                    if (_instance != null)
                        return _instance;
                    _instance = new ItemAuthorizer();
                }
                return _instance;
            }
        }

        private ItemAuthorizer()
        {

        }

        public bool IsAuthorized(IIdentity identity, IAuthorizableItem item)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var identityUser = userManager.FindById(identity.GetUserId());
            if (identityUser == null) 
                return false;
            var dbUser = FileSyncDal.Instance.GetDbUser(identityUser.Id);
            if (dbUser == null)
                return false;

            if (userManager.IsInRole(dbUser.Id, "Admin")) 
                return true;

            if (CheckIfItemAllowed(dbUser, item))
                return true;

            return CheckIfItemParentFoldersAllowed(dbUser, item.ParentFolder);
        }

        private bool CheckIfItemAllowed(FileSyncUser user, IAuthorizableItem item)
        {
            if (item == null || user == null) 
                return false;

            if (item.AuthorizedUsers.Any(u => u.Id == user.Id))
                return true;

            return CheckIfUserGroupsAllowed(user.UserGroups, item, new List<Group>());
        }

        private bool CheckIfItemParentFoldersAllowed(FileSyncUser user, Folder parentFolder)
        {
            if (user == null || parentFolder == null)
                return false;

            if (CheckIfItemAllowed(user, parentFolder))
                return true;

            return CheckIfItemParentFoldersAllowed(user, parentFolder.ParentFolder);
        }

        private bool CheckIfUserGroupsAllowed(ICollection<Group> groups, IAuthorizableItem item, ICollection<Group> checkedGroups)
        {
            if (item == null || groups == null) 
                return false;

            var nextGenerationGroups = new List<Group>();
            foreach (var group in groups)
            {
                checkedGroups.Add(group); // for deny of infinite loop situation

                if (item.AuthorizedGroups.Any(g => g.Id == group.Id))
                    return true;

                foreach(var parentGroup in group.ParentGroups)
                {
                    if (!checkedGroups.Any(g => g.Id == parentGroup.Id))
                        nextGenerationGroups.Add(parentGroup);
                }
            }

            if (nextGenerationGroups.Any())
                return CheckIfUserGroupsAllowed(nextGenerationGroups, item, checkedGroups);
            return false;
        }
    }
}