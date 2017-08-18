namespace SF.Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MgrBizAdmin",
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
                "dbo.MgrSysAdmin",
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
                        ScopeId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Icon = c.String(maxLength: 100),
                        Entity = c.String(nullable: false, maxLength: 100),
                        PasswordHash = c.String(nullable: false, maxLength: 100),
                        ObjectState = c.Byte(nullable: false),
                        SecurityStamp = c.String(nullable: false, maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedTime = c.DateTime(nullable: false),
                        SignupIdentProviderId = c.Long(nullable: false),
                        SignupIdentValue = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ScopeId)
                .Index(t => t.Entity)
                .Index(t => t.CreatedTime)
                .Index(t => t.SignupIdentProviderId)
                .Index(t => t.SignupIdentValue);
            
            CreateTable(
                "dbo.SysAuthIdentityCredential",
                c => new
                    {
                        ScopeId = c.Long(nullable: false),
                        ProviderId = c.Long(nullable: false),
                        Credential = c.String(nullable: false, maxLength: 100),
                        IdentityId = c.Long(nullable: false),
                        UnionIdent = c.String(maxLength: 100),
                        CreatedTime = c.DateTime(nullable: false),
                        ConfirmedTime = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.ScopeId, t.ProviderId, t.Credential })
                .ForeignKey("dbo.SysAuthIdentity", t => t.IdentityId)
                .Index(t => new { t.ScopeId, t.ProviderId, t.UnionIdent }, name: "union")
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
            
            CreateTable(
                "dbo.MgrMenu",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        ScopeId = c.Long(),
                        Ident = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 100),
                        ObjectState = c.Byte(nullable: false),
                        OwnerId = c.Long(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatorId = c.Long(nullable: false),
                        UpdatedTime = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.ScopeId, t.Ident }, name: "ident")
                .Index(t => t.Name)
                .Index(t => t.OwnerId)
                .Index(t => t.CreatedTime)
                .Index(t => t.UpdatorId);
            
            CreateTable(
                "dbo.MgrMenuItem",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        FontIcon = c.String(maxLength: 100),
                        Action = c.Int(nullable: false),
                        ActionArgument = c.String(maxLength: 200),
                        MenuId = c.Long(nullable: false),
                        ParentId = c.Long(),
                        ServiceId = c.Long(),
                        Title = c.String(nullable: false, maxLength: 100),
                        SubTitle = c.String(maxLength: 100),
                        Remarks = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        Memo = c.String(maxLength: 200),
                        Image = c.String(maxLength: 100),
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
                .ForeignKey("dbo.MgrMenuItem", t => t.ParentId)
                .ForeignKey("dbo.MgrMenu", t => t.MenuId)
                .Index(t => t.MenuId)
                .Index(t => t.ParentId)
                .Index(t => t.Name)
                .Index(t => t.ScopeId)
                .Index(t => t.OwnerId)
                .Index(t => t.CreatedTime)
                .Index(t => t.UpdatorId);
            
            CreateTable(
                "dbo.SysIdentSeed",
                c => new
                    {
                        Type = c.String(nullable: false, maxLength: 100),
                        NextValue = c.Long(nullable: false),
                        Section = c.Int(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Type);
            
            CreateTable(
                "dbo.SysServiceInstance",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        ParentId = c.Long(),
                        ServiceType = c.String(nullable: false, maxLength: 100),
                        ImplementType = c.String(nullable: false, maxLength: 300),
                        Priority = c.Int(nullable: false),
                        ServiceIdent = c.String(maxLength: 200),
                        Setting = c.String(),
                        Title = c.String(nullable: false, maxLength: 100),
                        SubTitle = c.String(maxLength: 100),
                        Remarks = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        Memo = c.String(maxLength: 200),
                        Image = c.String(maxLength: 100),
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
                .ForeignKey("dbo.SysServiceInstance", t => t.ParentId)
                .Index(t => new { t.ParentId, t.ServiceType }, name: "TypedService")
                .Index(t => t.ImplementType, name: "impl")
                .Index(t => t.ServiceIdent)
                .Index(t => t.Name)
                .Index(t => t.ScopeId)
                .Index(t => t.OwnerId)
                .Index(t => t.CreatedTime)
                .Index(t => t.UpdatorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SysServiceInstance", "ParentId", "dbo.SysServiceInstance");
            DropForeignKey("dbo.MgrMenuItem", "MenuId", "dbo.MgrMenu");
            DropForeignKey("dbo.MgrMenuItem", "ParentId", "dbo.MgrMenuItem");
            DropForeignKey("dbo.SysAuthIdentityCredential", "IdentityId", "dbo.SysAuthIdentity");
            DropIndex("dbo.SysServiceInstance", new[] { "UpdatorId" });
            DropIndex("dbo.SysServiceInstance", new[] { "CreatedTime" });
            DropIndex("dbo.SysServiceInstance", new[] { "OwnerId" });
            DropIndex("dbo.SysServiceInstance", new[] { "ScopeId" });
            DropIndex("dbo.SysServiceInstance", new[] { "Name" });
            DropIndex("dbo.SysServiceInstance", new[] { "ServiceIdent" });
            DropIndex("dbo.SysServiceInstance", "impl");
            DropIndex("dbo.SysServiceInstance", "TypedService");
            DropIndex("dbo.MgrMenuItem", new[] { "UpdatorId" });
            DropIndex("dbo.MgrMenuItem", new[] { "CreatedTime" });
            DropIndex("dbo.MgrMenuItem", new[] { "OwnerId" });
            DropIndex("dbo.MgrMenuItem", new[] { "ScopeId" });
            DropIndex("dbo.MgrMenuItem", new[] { "Name" });
            DropIndex("dbo.MgrMenuItem", new[] { "ParentId" });
            DropIndex("dbo.MgrMenuItem", new[] { "MenuId" });
            DropIndex("dbo.MgrMenu", new[] { "UpdatorId" });
            DropIndex("dbo.MgrMenu", new[] { "CreatedTime" });
            DropIndex("dbo.MgrMenu", new[] { "OwnerId" });
            DropIndex("dbo.MgrMenu", new[] { "Name" });
            DropIndex("dbo.MgrMenu", "ident");
            DropIndex("dbo.CommonTextMessageRecord", new[] { "ScopeId" });
            DropIndex("dbo.CommonTextMessageRecord", "service");
            DropIndex("dbo.CommonTextMessageRecord", "user");
            DropIndex("dbo.CommonTextMessageRecord", "status");
            DropIndex("dbo.SysAuthIdentityCredential", new[] { "IdentityId" });
            DropIndex("dbo.SysAuthIdentityCredential", "union");
            DropIndex("dbo.SysAuthIdentity", new[] { "SignupIdentValue" });
            DropIndex("dbo.SysAuthIdentity", new[] { "SignupIdentProviderId" });
            DropIndex("dbo.SysAuthIdentity", new[] { "CreatedTime" });
            DropIndex("dbo.SysAuthIdentity", new[] { "Entity" });
            DropIndex("dbo.SysAuthIdentity", new[] { "ScopeId" });
            DropIndex("dbo.MgrSysAdmin", new[] { "UpdatorId" });
            DropIndex("dbo.MgrSysAdmin", new[] { "CreatedTime" });
            DropIndex("dbo.MgrSysAdmin", new[] { "OwnerId" });
            DropIndex("dbo.MgrSysAdmin", new[] { "ScopeId" });
            DropIndex("dbo.MgrSysAdmin", new[] { "Name" });
            DropIndex("dbo.MgrSysAdmin", new[] { "Account" });
            DropIndex("dbo.MgrBizAdmin", new[] { "UpdatorId" });
            DropIndex("dbo.MgrBizAdmin", new[] { "CreatedTime" });
            DropIndex("dbo.MgrBizAdmin", new[] { "OwnerId" });
            DropIndex("dbo.MgrBizAdmin", new[] { "ScopeId" });
            DropIndex("dbo.MgrBizAdmin", new[] { "Name" });
            DropIndex("dbo.MgrBizAdmin", new[] { "Account" });
            DropTable("dbo.SysServiceInstance");
            DropTable("dbo.SysIdentSeed");
            DropTable("dbo.MgrMenuItem");
            DropTable("dbo.MgrMenu");
            DropTable("dbo.CommonTextMessageRecord");
            DropTable("dbo.SysAuthIdentityCredential");
            DropTable("dbo.SysAuthIdentity");
            DropTable("dbo.MgrSysAdmin");
            DropTable("dbo.MgrBizAdmin");
        }
    }
}
