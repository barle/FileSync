using FileSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace FileSync.Authorization
{
    public class ItemAuthorizer
    {
        public static bool IsAuthorized(IIdentity identity, IAuthorizableItem item)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(identity.GetUserId());
            if (user == null) 
                return false;

            if (userManager.IsInRole(user.Id, "Admin")) 
                return true;

            if (CheckIfItemAllowed(user, item))
                return true;

            return CheckIfItemParentFoldersAllowed(user, item.ParentFolder);
        }


        private static bool CheckIfItemAllowed(FileSyncUser user, IAuthorizableItem item)
        {
            if (item == null || user == null) 
                return false;

            if (item.AuthorizedUsers.Any(u => u.Id == user.Id))
                return true;

            return CheckIfUserGroupsAllowed(user.UserGroups, item, new List<Group>());
        }

        private static bool CheckIfItemParentFoldersAllowed(FileSyncUser user, Folder parentFolder)
        {
            if (user == null || parentFolder == null)
                return false;

            if (CheckIfItemAllowed(user, parentFolder))
                return true;

            return CheckIfItemParentFoldersAllowed(user, parentFolder.ParentFolder);
        }

        private static bool CheckIfUserGroupsAllowed(ICollection<Group> groups, IAuthorizableItem item, ICollection<Group> checkedGroups)
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