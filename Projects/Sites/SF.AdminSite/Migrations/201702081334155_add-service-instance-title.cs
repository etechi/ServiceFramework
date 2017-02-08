namespace SF.AdminSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addserviceinstancetitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SysServiceInstance", "Title", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SysServiceInstance", "Title");
        }
    }
}
