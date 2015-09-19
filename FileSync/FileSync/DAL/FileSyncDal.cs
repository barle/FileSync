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

        public static File GetFile(IIdentity identity, int fileId)
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

        public static Folder GetFolder(IIdentity identity, int folderId)
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

        public static void SaveEditFolder(Folder folder)
        {
            using (var db = new FileSyncDbContext())
            {
                db.Entry(folder).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void RemoveFolder(Folder folder)
        {
            using (var db = new FileSyncDbContext())
            {
                db.Folders.Remove(folder);
                db.SaveChanges();
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

        public static void AddUserToGroup(string groupId, FileSyncUser user)
        {
            if (user == null)
                return;

            var group = GetGroup(groupId);
            if (group == null)
                return;

            group.Users.Add(user);
            SaveEditGroup(group);
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

        public static void AddGroup(Group group)
        {
            using (var db = new FileSyncDbContext())
            {
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

        public static void RemoveGroup(Group group)
        {
            using (var db = new FileSyncDbContext())
            {
                db.Groups.Remove(group);
                db.SaveChanges();
            }
        }

        private static IEnumerable<T> GetAuthorized<T>(IIdentity identity, IEnumerable<T> itemsToAuthorize) where T : IAuthorizableItem
        {
            if (identity == null) return Enumerable.Empty<T>();

            var authorizedItems = itemsToAuthorize.Where(item => ItemAuthorizer.IsAuthorized(identity, item)).ToList();
            return itemsToAuthorize;
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