namespace FileSync.Migrations
{
    using FileSync.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FileSync.Models.FileSyncDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "FileSync.Models.FileSyncDbContext";
        }

        protected override void Seed(FileSync.Models.FileSyncDbContext context)
        {
            if (System.Diagnostics.Debugger.IsAttached == false)
                System.Diagnostics.Debugger.Launch();

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }

            if (!context.Users.Any(u => u.UserName == "Admin"))
            {
                var store = new UserStore<FileSyncUser>(context);
                var manager = new ApplicationUserManager(store);
                var user = new FileSyncUser { UserName = "Admin", Email = "Admin@Admin.com", Place = "AdminIsrael" };
                manager.Create(user, "Administrator");
                var result = manager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
