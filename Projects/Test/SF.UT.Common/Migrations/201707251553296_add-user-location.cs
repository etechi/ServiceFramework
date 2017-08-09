namespace SF.Core.net46.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adduserlocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Location_City", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.User", "Location_Address", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "Location_Address");
            DropColumn("dbo.User", "Location_City");
        }
    }
}
