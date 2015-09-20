﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileSync.Models
{
    public class LinkGroupViewModel
    {
        public LinkGroupViewModel()
        {
            Groups = new List<Group>();
        }
        public IEnumerable<Group> Groups { get; set; }
        public string CallbackAction { get; set; }
        public string CallbackController { get; set; }
        public string ParentId { get; set; }
    }
}