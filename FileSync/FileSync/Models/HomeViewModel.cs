using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileSync.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            Folders = new List<Folder>();
            Files = new List<File>();
        }
        public IEnumerable<Folder> Folders { get; set; }
        public IEnumerable<File> Files { get; set; }
        public Folder ParentFolder { get; set; }
    }
}