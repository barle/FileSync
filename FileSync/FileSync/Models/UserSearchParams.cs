using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileSync.Models
{
    public class UserSearchParams
    {
        public UserSearchParams(string name, string groupName, int? groupsCount)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "" : name;
            GroupName = string.IsNullOrWhiteSpace(groupName) ? "" : groupName;
            GroupsCount = groupsCount.HasValue ? groupsCount.Value : 0;
        }

        public string Name { get; set; }
        public string GroupName { get; set; }
        public int GroupsCount { get; set; }
    }
}