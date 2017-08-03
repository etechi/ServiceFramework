namespace SF.AdminSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adjustimpltypelength : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SysServiceInstance", "impl");
            AlterColumn("dbo.SysServiceInstance", "ImplementType", c => c.String(nullable: false, maxLength: 300));
            CreateIndex("dbo.SysServiceInstance", "ImplementType", name: "impl");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SysServiceInstance", "impl");
            AlterColumn("dbo.SysServiceInstance", "ImplementType", c => c.String(nullable: false, maxLength: 200));
            CreateIndex("dbo.SysServiceInstance", "ImplementType", name: "impl");
        }
    }
}
