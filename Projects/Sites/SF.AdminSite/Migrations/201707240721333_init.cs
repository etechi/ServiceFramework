using System;
using System.Data.Entity.Migrations;
namespace SF.AdminSite.Migrations
{
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SysIdentSeed",
                c => new
                    {
                        ScopeId = c.Long(nullable: false),
                        Type = c.String(nullable: false, maxLength: 100),
                        NextValue = c.Long(nullable: false),
                        Section = c.Int(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.ScopeId, t.Type });
            
            CreateTable(
                "dbo.SysServiceInstance",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        ParentId = c.Long(),
                        ServiceType = c.String(nullable: false, maxLength: 100),
                        ImplementType = c.String(nullable: false, maxLength: 100),
                        Priority = c.Int(nullable: false),
                        ServiceIdent = c.String(maxLength: 100),
                        Setting = c.String(),
                        Title = c.String(nullable: false, maxLength: 100),
                        SubTitle = c.String(maxLength: 100),
                        Remarks = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        Memo = c.String(maxLength: 200),
                        Image = c.String(maxLength: 100),
                        Icon = c.String(maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 100),
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
                .Index(t => t.OwnerId)
                .Index(t => t.CreatedTime)
                .Index(t => t.UpdatorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SysServiceInstance", "ParentId", "dbo.SysServiceInstance");
            DropIndex("dbo.SysServiceInstance", new[] { "UpdatorId" });
            DropIndex("dbo.SysServiceInstance", new[] { "CreatedTime" });
            DropIndex("dbo.SysServiceInstance", new[] { "OwnerId" });
            DropIndex("dbo.SysServiceInstance", new[] { "Name" });
            DropIndex("dbo.SysServiceInstance", new[] { "ServiceIdent" });
            DropIndex("dbo.SysServiceInstance", "impl");
            DropIndex("dbo.SysServiceInstance", "TypedService");
            DropTable("dbo.SysServiceInstance");
            DropTable("dbo.SysIdentSeed");
        }
    }
}
