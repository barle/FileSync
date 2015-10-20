using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FileSync.Models
{
    public class Folder : IAuthorizableItem
    {
        public Folder()
        {
            AuthorizedUsers = new List<FileSyncUser>();
            AuthorizedGroups = new List<Group>();
            SubFolders = new List<Folder>();
            Files = new List<File>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual Folder ParentFolder { get; set; }
        public string Path { get; set; }

        [DisplayName("Parent Folder")]
        public string ParentFolderId { get; set; }

        public DateTime InsertionDate { get; set; }

        public long Size { get; set; }

        public virtual ICollection<Folder> SubFolders { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual ICollection<FileSyncUser> AuthorizedUsers { get; set; }
        public virtual ICollection<Group> AuthorizedGroups { get; set; }
    }
}