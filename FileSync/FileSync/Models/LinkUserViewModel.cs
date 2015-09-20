﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileSync.Models
{
    public class LinkUserViewModel
    {
        public LinkUserViewModel()
        {
            Users = new List<FileSyncUser>();
        }
        public IEnumerable<FileSyncUser> Users { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ParentId { get; set; }
    }
}