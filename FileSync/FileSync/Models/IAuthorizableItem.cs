using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Models
{
    public interface IAuthorizableItem
    {
        Folder ParentFolder { get; set; }
        ICollection<FileSyncUser> AuthorizedUsers { get; set; }
        ICollection<Group> AuthorizedGroups { get; set; }
    }
}
