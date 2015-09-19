using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileSync.Models
{
    public class File : IAuthorizableItem
    {
        public File()
        {
            AuthorizedUsers = new List<FileSyncUser>();
            AuthorizedGroups = new List<Group>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public Folder ParentFolder { get; set; }

        public int ParentFolderId { get; set; }
        public ICollection<FileSyncUser> AuthorizedUsers { get; set; }
        public ICollection<Group> AuthorizedGroups { get; set; }
    }
}