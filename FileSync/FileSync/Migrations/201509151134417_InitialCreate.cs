namespace FileSync.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IdentityRole_Id = c.String(maxLength: 128),
                        FileSyncUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.Role", t => t.IdentityRole_Id)
                .ForeignKey("dbo.User", t => t.FileSyncUser_Id)
                .Index(t => t.IdentityRole_Id)
                .Index(t => t.FileSyncUser_Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        FileSyncUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.FileSyncUser_Id)
                .Index(t => t.FileSyncUser_Id);
            
            CreateTable(
                "dbo.UserLogin",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        FileSyncUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.User", t => t.FileSyncUser_Id)
                .Index(t => t.FileSyncUser_Id);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DisplayName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupToSubGroup",
                c => new
                    {
                        GroupId = c.String(nullable: false, maxLength: 128),
                        SubGroupId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.GroupId, t.SubGroupId })
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.Groups", t => t.SubGroupId)
                .Index(t => t.GroupId)
                .Index(t => t.SubGroupId);
            
            CreateTable(
                "dbo.GroupToUser",
                c => new
                    {
                        GroupId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.GroupId, t.UserId })
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupToUser", "UserId", "dbo.User");
            DropForeignKey("dbo.GroupToUser", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.GroupToSubGroup", "SubGroupId", "dbo.Groups");
            DropForeignKey("dbo.GroupToSubGroup", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.UserRole", "FileSyncUser_Id", "dbo.User");
            DropForeignKey("dbo.UserLogin", "FileSyncUser_Id", "dbo.User");
            DropForeignKey("dbo.UserClaim", "FileSyncUser_Id", "dbo.User");
            DropForeignKey("dbo.UserRole", "IdentityRole_Id", "dbo.Role");
            DropIndex("dbo.GroupToUser", new[] { "UserId" });
            DropIndex("dbo.GroupToUser", new[] { "GroupId" });
            DropIndex("dbo.GroupToSubGroup", new[] { "SubGroupId" });
            DropIndex("dbo.GroupToSubGroup", new[] { "GroupId" });
            DropIndex("dbo.UserLogin", new[] { "FileSyncUser_Id" });
            DropIndex("dbo.UserClaim", new[] { "FileSyncUser_Id" });
            DropIndex("dbo.UserRole", new[] { "FileSyncUser_Id" });
            DropIndex("dbo.UserRole", new[] { "IdentityRole_Id" });
            DropTable("dbo.GroupToUser");
            DropTable("dbo.GroupToSubGroup");
            DropTable("dbo.Groups");
            DropTable("dbo.UserLogin");
            DropTable("dbo.UserClaim");
            DropTable("dbo.User");
            DropTable("dbo.UserRole");
            DropTable("dbo.Role");
        }
    }
}
