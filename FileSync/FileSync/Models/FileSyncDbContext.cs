using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace FileSync.Models
{
    public class FileSyncDbContext : IdentityDbContext<FileSyncUser>
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Folder> Folders { get; set; }

        public FileSyncDbContext()
            : base("FileSyncConnection", throwIfV1Schema: false)
        {
        }

        public static FileSyncDbContext Create()
        {
            return new FileSyncDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            SetTablesNames(modelBuilder);
            SetTablesKeys(modelBuilder);
            SetRelations(modelBuilder);
        }

        private void SetTablesNames(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileSyncUser>().ToTable("User");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<IdentityRole>().ToTable("Role");
        }

        private void SetTablesKeys(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
        }

        private void SetRelations(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
                        .HasMany<Group>(group => group.SubGroups)
                        .WithMany(group => group.ParentGroups)
                        .Map(groupToSubGroup =>
                        {
                            groupToSubGroup.MapLeftKey("GroupId");
                            groupToSubGroup.MapRightKey("SubGroupId");
                            groupToSubGroup.ToTable("GroupToSubGroup");
                        });

            modelBuilder.Entity<Group>()
                        .HasMany<FileSyncUser>(group => group.Users)
                        .WithMany(user => user.UserGroups)
                        .Map(groupToUser =>
                        {
                            groupToUser.MapLeftKey("GroupId");
                            groupToUser.MapRightKey("UserId");
                            groupToUser.ToTable("GroupToUser");
                        });

            modelBuilder.Entity<Folder>()
                .HasMany<File>(folder => folder.Files)
                .WithRequired(file => file.ParentFolder)
                .HasForeignKey(file => file.ParentFolderId);

            modelBuilder.Entity<Folder>()
                .HasMany<Folder>(parentFolder => parentFolder.SubFolders)
                .WithOptional(folder => folder.ParentFolder)
                .HasForeignKey(folder => folder.ParentFolderId);

            modelBuilder.Entity<Folder>()
                .HasMany<FileSyncUser>(folder => folder.AuthorizedUsers)
                .WithMany(user => user.AllowedFolders)
                .Map(folerToAllowedUser =>
                {
                    folerToAllowedUser.MapLeftKey("FolderId");
                    folerToAllowedUser.MapRightKey("UserId");
                    folerToAllowedUser.ToTable("FolderAllowedUsers");
                });

            modelBuilder.Entity<Folder>()
                .HasMany<Group>(folder => folder.AuthorizedGroups)
                .WithMany(group => group.AllowedFolders)
                .Map(folderToAllowedGroup =>
                {
                    folderToAllowedGroup.MapLeftKey("FolderId");
                    folderToAllowedGroup.MapRightKey("GroupId");
                    folderToAllowedGroup.ToTable("FolderAllowedGroups");
                });


            modelBuilder.Entity<File>()
                .HasMany<FileSyncUser>(file => file.AuthorizedUsers)
                .WithMany(user => user.AllowedFiles)
                .Map(fileToAllowedUser =>
                {
                    fileToAllowedUser.MapLeftKey("FileId");
                    fileToAllowedUser.MapRightKey("UserId");
                    fileToAllowedUser.ToTable("FileAllowedUsers");
                });

            modelBuilder.Entity<File>()
                .HasMany<Group>(file => file.AuthorizedGroups)
                .WithMany(group => group.AllowedFiles)
                .Map(folderToAllowedGroup =>
                {
                    folderToAllowedGroup.MapLeftKey("FileId");
                    folderToAllowedGroup.MapRightKey("GroupId");
                    folderToAllowedGroup.ToTable("FileAllowedGroups");
                });
        }
    }
}