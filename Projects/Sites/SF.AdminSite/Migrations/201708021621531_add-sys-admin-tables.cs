namespace SF.AdminSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsysadmintables : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.MgrMenuItem", name: "ident", newName: "IX_ScopeId");
            CreateTable(
                "dbo.MgrAdmin",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Account = c.String(maxLength: 100),
                        Icon = c.String(maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 100),
                        ScopeId = c.Long(),
                        ObjectState = c.Byte(nullable: false),
                        OwnerId = c.Long(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatorId = c.Long(nullable: false),
                        UpdatedTime = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Account)
                .Index(t => t.Name)
                .Index(t => t.ScopeId)
                .Index(t => t.OwnerId)
                .Index(t => t.CreatedTime)
                .Index(t => t.UpdatorId);
            
            CreateTable(
                "dbo.SysAuthIdentity",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        AppId = c.Long(nullable: false),
                        ScopeId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Icon = c.String(maxLength: 100),
                        Entity = c.String(nullable: false, maxLength: 100),
                        PasswordHash = c.String(nullable: false, maxLength: 100),
                        ObjectState = c.Byte(nullable: false),
                        SecurityStamp = c.String(nullable: false, maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(nullable: false),
                        SignupIdentProvider = c.Long(nullable: false),
                        SignupIdentValue = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AppId)
                .Index(t => t.ScopeId)
                .Index(t => t.Entity)
                .Index(t => t.CreatedTime)
                .Index(t => t.SignupIdentProvider)
                .Index(t => t.SignupIdentValue);
            
            CreateTable(
                "dbo.SysAuthIdentityCredential",
                c => new
                    {
                        ScopeId = c.Long(nullable: false),
                        ProviderId = c.Long(nullable: false),
                        Credential = c.String(nullable: false, maxLength: 100),
                        AppId = c.Long(nullable: false),
                        IdentityId = c.Long(nullable: false),
                        UnionIdent = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        ConfirmedTime = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.ScopeId, t.ProviderId, t.Credential })
                .ForeignKey("dbo.SysAuthIdentity", t => t.IdentityId)
                .Index(t => new { t.ScopeId, t.ProviderId, t.UnionIdent }, name: "union")
                .Index(t => t.AppId)
                .Index(t => t.IdentityId);
            
            CreateTable(
                "dbo.CommonTextMessageRecord",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        UserId = c.Long(),
                        ServiceId = c.Long(nullable: false),
                        Sender = c.String(maxLength: 100),
                        Target = c.String(nullable: false),
                        Title = c.String(maxLength: 100),
                        Body = c.String(maxLength: 1000),
                        Headers = c.String(maxLength: 1000),
                        Args = c.String(maxLength: 1000),
                        Time = c.DateTime(nullable: false),
                        CompletedTime = c.DateTime(),
                        Error = c.String(),
                        Result = c.String(),
                        TrackEntityId = c.String(maxLength: 100),
                        ScopeId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Status, name: "status")
                .Index(t => t.UserId, name: "user")
                .Index(t => new { t.ServiceId, t.Time }, name: "service")
                .Index(t => t.ScopeId);
            
            AddColumn("dbo.SysServiceInstance", "ScopeId", c => c.Long());
            CreateIndex("dbo.SysServiceInstance", "ScopeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SysAuthIdentityCredential", "IdentityId", "dbo.SysAuthIdentity");
            DropIndex("dbo.SysServiceInstance", new[] { "ScopeId" });
            DropIndex("dbo.CommonTextMessageRecord", new[] { "ScopeId" });
            DropIndex("dbo.CommonTextMessageRecord", "service");
            DropIndex("dbo.CommonTextMessageRecord", "user");
            DropIndex("dbo.CommonTextMessageRecord", "status");
            DropIndex("dbo.SysAuthIdentityCredential", new[] { "IdentityId" });
            DropIndex("dbo.SysAuthIdentityCredential", new[] { "AppId" });
            DropIndex("dbo.SysAuthIdentityCredential", "union");
            DropIndex("dbo.SysAuthIdentity", new[] { "SignupIdentValue" });
            DropIndex("dbo.SysAuthIdentity", new[] { "SignupIdentProvider" });
            DropIndex("dbo.SysAuthIdentity", new[] { "CreatedTime" });
            DropIndex("dbo.SysAuthIdentity", new[] { "Entity" });
            DropIndex("dbo.SysAuthIdentity", new[] { "ScopeId" });
            DropIndex("dbo.SysAuthIdentity", new[] { "AppId" });
            DropIndex("dbo.MgrAdmin", new[] { "UpdatorId" });
            DropIndex("dbo.MgrAdmin", new[] { "CreatedTime" });
            DropIndex("dbo.MgrAdmin", new[] { "OwnerId" });
            DropIndex("dbo.MgrAdmin", new[] { "ScopeId" });
            DropIndex("dbo.MgrAdmin", new[] { "Name" });
            DropIndex("dbo.MgrAdmin", new[] { "Account" });
            DropColumn("dbo.SysServiceInstance", "ScopeId");
            DropTable("dbo.CommonTextMessageRecord");
            DropTable("dbo.SysAuthIdentityCredential");
            DropTable("dbo.SysAuthIdentity");
            DropTable("dbo.MgrAdmin");
            RenameIndex(table: "dbo.MgrMenuItem", name: "IX_ScopeId", newName: "ident");
        }
    }
}
