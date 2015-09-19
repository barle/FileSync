namespace FileSync.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FileSync.Models.FileSyncDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "FileSync.Models.FileSyncDbContext";
        }

        protected override void Seed(FileSync.Models.FileSyncDbContext context)
        {
            var adminRole = context.Roles.Create();
            adminRole.Id = "Admin";
            adminRole.Name = "Admin";
            context.Roles.AddOrUpdate(adminRole);
            context.SaveChanges();
        }
    }
}
