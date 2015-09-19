using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel;

namespace FileSync.Models
{
    public class FileSyncUser : IdentityUser
    {
        public FileSyncUser() : base()
        {
            UserGroups = new List<Group>();
            AllowedFiles = new List<File>();
            AllowedFolders = new List<Folder>();
        }
        public ICollection<Group> UserGroups { get; set; }

        public ICollection<File> AllowedFiles { get; set; }

        public ICollection<Folder> AllowedFolders { get; set; }

        [DisplayName("Address")]
        public string Place { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<FileSyncUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}