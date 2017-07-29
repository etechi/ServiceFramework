namespace SF.AdminSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmenuservice : DbMigration
    {
        public override void Up()
        {
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
                        ScopeId = c.Long(),
                        Id = c.Long(nullable: false),
                        FontIcon = c.String(maxLength: 100),
                        Action = c.Int(nullable: false),
                        ActionArgument = c.String(maxLength: 200),
                        MenuId = c.Long(nullable: false),
                        ParentId = c.Long(),
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
                .ForeignKey("dbo.MgrMenuItem", t => t.ParentId)
                .ForeignKey("dbo.MgrMenu", t => t.MenuId)
                .Index(t => t.ScopeId, name: "ident")
                .Index(t => t.MenuId)
                .Index(t => t.ParentId)
                .Index(t => t.Name)
                .Index(t => t.OwnerId)
                .Index(t => t.CreatedTime)
                .Index(t => t.UpdatorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MgrMenuItem", "MenuId", "dbo.MgrMenu");
            DropForeignKey("dbo.MgrMenuItem", "ParentId", "dbo.MgrMenuItem");
            DropIndex("dbo.MgrMenuItem", new[] { "UpdatorId" });
            DropIndex("dbo.MgrMenuItem", new[] { "CreatedTime" });
            DropIndex("dbo.MgrMenuItem", new[] { "OwnerId" });
            DropIndex("dbo.MgrMenuItem", new[] { "Name" });
            DropIndex("dbo.MgrMenuItem", new[] { "ParentId" });
            DropIndex("dbo.MgrMenuItem", new[] { "MenuId" });
            DropIndex("dbo.MgrMenuItem", "ident");
            DropIndex("dbo.MgrMenu", new[] { "UpdatorId" });
            DropIndex("dbo.MgrMenu", new[] { "CreatedTime" });
            DropIndex("dbo.MgrMenu", new[] { "OwnerId" });
            DropIndex("dbo.MgrMenu", new[] { "Name" });
            DropIndex("dbo.MgrMenu", "ident");
            DropTable("dbo.MgrMenuItem");
            DropTable("dbo.MgrMenu");
        }
    }
}
