namespace SF.AdminSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adjusttypelength : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SysServiceInstance", "impl");
            DropIndex("dbo.SysServiceInstance", new[] { "ServiceIdent" });
            AlterColumn("dbo.SysServiceInstance", "ImplementType", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.SysServiceInstance", "ServiceIdent", c => c.String(maxLength: 200));
            CreateIndex("dbo.SysServiceInstance", "ImplementType", name: "impl");
            CreateIndex("dbo.SysServiceInstance", "ServiceIdent");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SysServiceInstance", new[] { "ServiceIdent" });
            DropIndex("dbo.SysServiceInstance", "impl");
            AlterColumn("dbo.SysServiceInstance", "ServiceIdent", c => c.String(maxLength: 100));
            AlterColumn("dbo.SysServiceInstance", "ImplementType", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.SysServiceInstance", "ServiceIdent");
            CreateIndex("dbo.SysServiceInstance", "ImplementType", name: "impl");
        }
    }
}
