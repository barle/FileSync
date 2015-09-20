using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FileSync.Models
{
    public class Group
    {
        public Group()
        {
            Users = new List<FileSyncUser>();
            SubGroups = new List<Group>();
            ParentGroups = new List<Group>();
            AllowedFolders = new List<Folder>();
            AllowedFiles = new List<File>();
        }
        public string Id { get; set; }
        [DisplayName("Group Name")]
        public string DisplayName { get; set; }
        public ICollection<FileSyncUser> Users { get; set; }
        public ICollection<Group> SubGroups { get; set; }
        public ICollection<Group> ParentGroups { get; set; }
        public ICollection<File> AllowedFiles { get; set; }
        public ICollection<Folder> AllowedFolders { get; set; }
    }
}