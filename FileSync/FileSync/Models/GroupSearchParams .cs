using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileSync.Models
{
    public class GroupSearchParams
    {
        public GroupSearchParams(string name, string userName, int? membersCount)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "" : name;
            UserName = string.IsNullOrWhiteSpace(userName) ? "" : userName;
            MembersCount = membersCount.HasValue ? membersCount.Value : 0;
        }

        public string Name { get; set; }
        public string UserName { get; set; }
        public int MembersCount { get; set; }
    }
}