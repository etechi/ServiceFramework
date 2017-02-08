namespace SF.AdminSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addserviceinstancefields : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SysServiceInstance", "service");
            DropIndex("dbo.SysServiceInstance", new[] { "ImplementId" });
            AddColumn("dbo.SysServiceInstance", "Description", c => c.String(maxLength: 200));
            AddColumn("dbo.SysServiceInstance", "Remarks", c => c.String(maxLength: 200));
            AddColumn("dbo.SysServiceInstance", "Image", c => c.String(maxLength: 100));
            AddColumn("dbo.SysServiceInstance", "Icon", c => c.String(maxLength: 100));
            AlterColumn("dbo.SysServiceInstance", "DeclarationId", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.SysServiceInstance", "ImplementId", c => c.String(nullable: false, maxLength: 200));
            CreateIndex("dbo.SysServiceInstance", new[] { "DeclarationId", "IsDefaultService" }, name: "service");
            CreateIndex("dbo.SysServiceInstance", "ImplementId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SysServiceInstance", new[] { "ImplementId" });
            DropIndex("dbo.SysServiceInstance", "service");
            AlterColumn("dbo.SysServiceInstance", "ImplementId", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.SysServiceInstance", "DeclarationId", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.SysServiceInstance", "Icon");
            DropColumn("dbo.SysServiceInstance", "Image");
            DropColumn("dbo.SysServiceInstance", "Remarks");
            DropColumn("dbo.SysServiceInstance", "Description");
            CreateIndex("dbo.SysServiceInstance", "ImplementId");
            CreateIndex("dbo.SysServiceInstance", new[] { "DeclarationId", "IsDefaultService" }, name: "service");
        }
    }
}
