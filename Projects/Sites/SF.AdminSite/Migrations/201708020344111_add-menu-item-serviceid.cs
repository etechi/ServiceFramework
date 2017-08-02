namespace SF.AdminSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmenuitemserviceid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MgrMenuItem", "ServiceId", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MgrMenuItem", "ServiceId");
        }
    }
}
