using FileSync.Authorization;
using FileSync.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace FileSync.DAL
{
    public class FileSyncDal
    {
        const string FOLDER_INCLUDES = "Files,SubFolders,AuthorizedUsers,AuthorizedGroups,ParentFolder";
        const string FILE_INCLUDES = "AuthorizedUsers,AuthorizedGroups,ParentFolder";
        const string GROUP_INCLUDES = "Users,SubGroups,ParentGroups,AllowedFolders,AllowedFiles";

        public static IEnumerable<FileSyncUser> GetAllUsers()
        {
            using (var db = new FileSyncDbContext())
            {
                var users = (from u in db.Users
                             select u).ToList();
                return users;
            }
        }

        public static IEnumerable<FileSyncUser> GetUsersBySearch(UserSearchParams searchParams)
        {
            using (var db = new FileSyncDbContext())
            {
                var users = (from u in db.Users
                              from g in db.Groups.Where(g2 => u.UserGroups.Any(g3 => g2.Id == g3.Id)).DefaultIfEmpty()
                              where
                              ((searchParams.GroupName == null || searchParams.GroupName == string.Empty) || (g != null && g.DisplayName.Contains(searchParams.GroupName))) &&
                              (u.UserName.Contains(searchParams.Name))
                              group u by u into groupedUsers
                              where (groupedUsers.Key.UserGroups.Count() >= searchParams.GroupsCount)
                              select groupedUsers.Key
                                  ).ToList();
                return users;
            }
        }

        public static File GetFile(IIdentity identity, string fileId)
        {
            using(var db = new FileSyncDbContext())
            {
                var files = (from f in db.Files.Includes(FILE_INCLUDES)
                            where f.Id == fileId
                            select f).ToList();
                var authorizedFiles = GetAuthorized<File>(identity, files);
                return authorizedFiles.FirstOrDefault();
            }
        }

        public static Folder GetFolder(IIdentity identity, string folderId)
        {
            using (var db = new FileSyncDbContext())
            {
                var folders = (from f in db.Folders.Includes(FOLDER_INCLUDES)
                              where f.Id == folderId
                              select f).ToList();
                var authorizedFolders = GetAuthorized<Folder>(identity, folders);
                return authorizedFolders.FirstOrDefault();
            }
        }

        public static IEnumerable<File> GetAllFiles(IIdentity identity)
        {
            using (var db = new FileSyncDbContext())
            {
                var files = from file in db.Files.Includes(FILE_INCLUDES)
                            select file;
                return GetAuthorized<File>(identity, files.ToList());
            }
        }

        public static IEnumerable<Folder> GetAllFolders(IIdentity identity)
        {
            using (var db = new FileSyncDbContext())
            {
                var folders = from folder in db.Folders.Includes(FOLDER_INCLUDES)
                            select folder;
                return GetAuthorized<Folder>(identity, folders.ToList());
            }
        }

        public static IEnumerable<Folder> GetRootFolders(IIdentity identity)
        {
            using (var db = new FileSyncDbContext())
            {
                var rootFolders = (from f in db.Folders
                                   where f.ParentFolderId == null
                                   select f).ToList();
                var authorizedFolders = GetAuthorized<Folder>(identity, rootFolders);
                return authorizedFolders;
            }
        }

        public static void AddFile(File file)
        {
            using (var db = new FileSyncDbContext())
            {
                db.Files.Add(file);
                db.SaveChanges();
            }
        }

        public static void AddFiles(IEnumerable<File> files)
        {
            using (var db = new FileSyncDbContext())
            {
                db.Files.AddRange(files);
                db.SaveChanges();
            }
        }

        public static void SaveEditFile(File file)
        {
            using (var db = new FileSyncDbContext())
            {
                db.Entry(file).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void RemoveFile(File file)
        {
            using (var db = new FileSyncDbContext())
            {
                db.Files.Remove(file);
                db.SaveChanges();
            }
        }

        public static void AddFolder(Folder folder)
        {
            using (var db = new FileSyncDbContext())
            {
                db.Folders.Add(folder);
                db.SaveChanges();
            }
        }
        public static void AddFolders(IEnumerable<Folder> folders)
        {
            using (var db = new FileSyncDbContext())
            {
                db.Folders.AddRange(folders);
                db.SaveChanges();
            }
        }

        public static void SaveEditFolder(Folder folder)
        {
            using (var db = new FileSyncDbContext())
            {
                db.Entry(folder).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void RemoveFolder(string folderId)
        {
            using (var db = new FileSyncDbContext())
            {
                var folder = (from f in db.Folders where f.Id == folderId select f).FirstOrDefault();
                if (folder == null)
                    return;

                var allFolders = (from f in db.Folders select f).ToList();
                var foldersToRemove = new List<Folder>(){folder};
                GetAllFoldersInHirarchy(folder, allFolders, foldersToRemove);
                db.Folders.RemoveRange(foldersToRemove);
                db.SaveChanges();
            }
        }

        private static void GetAllFoldersInHirarchy(Folder folder, List<Folder> allFolders, List<Folder> foldersToRemove)
        {
            var subFolders = allFolders.Where(f => f.ParentFolderId == folder.Id).ToList();
            var diffedFolders = subFolders.Except(foldersToRemove);
            foreach (var f in diffedFolders)
            {
                if (!foldersToRemove.Contains(f))
                {
                    foldersToRemove.Add(f);
                    GetAllFoldersInHirarchy(f, allFolders, foldersToRemove);
                }
            }
        }

        public static void RemoveUserFromGroup(string groupId, string userId)
        {
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(userId))
                return;

            using (var db = new FileSyncDbContext())
            {
                var group = db.Groups.Find(groupId);
                if (group == null)
                    return;

                var user = db.Users.Find(userId);
                if (user == null)
                    return;

                db.Entry(group).Collection("Users").Load();

                group.Users.Remove(user);
                db.SaveChanges();
            }
        }

        public static void AddUserToGroup(string groupId, string userId)
        {
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(userId))
                return;

            using (var db = new FileSyncDbContext())
            {
                var group = db.Groups.Find(groupId);
                if (group == null)
                    return;

                var user = db.Users.Find(userId);
                if (user == null)
                    return;

                db.Entry(group).Collection("Users").Load();

                group.Users.Add(user);
                db.SaveChanges();
            }
        }

        public static void RemoveUserFromFolder(string folderId, string userId)
        {
            if (string.IsNullOrEmpty(folderId) || string.IsNullOrEmpty(userId))
                return;

            using (var db = new FileSyncDbContext())
            {
                var folder = db.Folders.Find(folderId);
                if (folder == null)
                    return;

                var user = db.Users.Find(userId);
                if (user == null)
                    return;

                db.Entry(folder).Collection("AuthorizedUsers").Load();

                folder.AuthorizedUsers.Remove(user);
                db.SaveChanges();
            }
        }

        public static void AddUserToFolder(string folderId, string userId)
        {
            if (string.IsNullOrEmpty(folderId) || string.IsNullOrEmpty(userId))
                return;

            using (var db = new FileSyncDbContext())
            {
                var folder = db.Folders.Find(folderId);
                if (folder == null)
                    return;

                var user = db.Users.Find(userId);
                if (user == null)
                    return;

                db.Entry(folder).Collection("AuthorizedUsers").Load();

                folder.AuthorizedUsers.Add(user);
                db.SaveChanges();
            }
        }

        public static void RemoveGroupFromFolder(string folderId, string groupId)
        {
            if (string.IsNullOrEmpty(folderId) || string.IsNullOrEmpty(groupId))
                return;

            using (var db = new FileSyncDbContext())
            {
                var folder = db.Folders.Find(folderId);
                if (folder == null)
                    return;

                var group = db.Groups.Find(groupId);
                if (group == null)
                    return;

                db.Entry(folder).Collection("AuthorizedGroups").Load();

                folder.AuthorizedGroups.Remove(group);
                db.SaveChanges();
            }
        }

        public static void AddGroupToFolder(string folderId, string groupId)
        {
            if (string.IsNullOrEmpty(folderId) || string.IsNullOrEmpty(groupId))
                return;

            using (var db = new FileSyncDbContext())
            {
                var folder = db.Folders.Find(folderId);
                if (folder == null)
                    return;

                var group = db.Groups.Find(groupId);
                if (group == null)
                    return;

                db.Entry(folder).Collection("AuthorizedGroups").Load();

                folder.AuthorizedGroups.Add(group);
                db.SaveChanges();
            }
        }

        public static Group GetGroup(string groupId)
        {
            using (var db = new FileSyncDbContext())
            {
                var group = (from g in db.Groups.Includes(GROUP_INCLUDES)
                             where g.Id == groupId
                             select g).FirstOrDefault();
                return group;
            }
        }

        public static IEnumerable<Group> GetAllGroups()
        {
            using (var db = new FileSyncDbContext())
            {
                var groups = (from g in db.Groups.Includes(GROUP_INCLUDES)
                              select g).ToList();
                return groups;
            }
        }

        public static IEnumerable<Group> GetGroupsBySearch(GroupSearchParams searchParams)
        {
            using (var db = new FileSyncDbContext())
            {
                var groups = (from g in db.Groups
                              from u in db.Users.Where(u2 => g.Users.Any(u3 => u2.Id == u3.Id)).DefaultIfEmpty()
                              where
                              ((searchParams.UserName == null || searchParams.UserName == string.Empty) || (u != null && u.UserName.Contains(searchParams.UserName))) &&
                              (g.DisplayName.Contains(searchParams.Name))
                              group g by g into groupedGroups
                              where (groupedGroups.Key.Users.Count() >= searchParams.MembersCount)
                              select groupedGroups.Key
                               ).ToList();
                return groups;
            }
        }

        public static void AddGroup(Group group)
        {
            using (var db = new FileSyncDbContext())
            {
                group.Id = Guid.NewGuid().ToString();
                db.Groups.Add(group);
                db.SaveChanges();
            }
        }

        public static void SaveEditGroup(Group group)
        {
            using (var db = new FileSyncDbContext())
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void RemoveGroup(string groupId)
        {
            using (var db = new FileSyncDbContext())
            {
                var group = (from g in db.Groups
                            where g.Id == groupId
                            select g).FirstOrDefault();
                if (group == null)
                    return;

                db.Groups.Remove(group);
                db.SaveChanges();
            }
        }

        private static IEnumerable<T> GetAuthorized<T>(IIdentity identity, IEnumerable<T> itemsToAuthorize) where T : IAuthorizableItem
        {
            if (identity == null) return Enumerable.Empty<T>();

            var authorizedItems = itemsToAuthorize.Where(item => ItemAuthorizer.IsAuthorized(identity, item)).ToList();
            return authorizedItems;
        }
    }


    internal static class Extentions
    {
        public static DbQuery<T> Includes<T>(this DbQuery<T> query, string pathes)
        {
            var returnQuery = query;
            foreach (var path in pathes.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                returnQuery = returnQuery.Include(path);
            }
            return returnQuery;
        }
    }
}