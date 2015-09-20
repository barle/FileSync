using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileSync.Models
{
    public class ShowVideoViewModel
    {
        public string VideoPath { get; set; }
        public string VideoName { get; set; }
        public string VideoType { get; set; }
        public string ReturnUrl { get; set; }
    }
}