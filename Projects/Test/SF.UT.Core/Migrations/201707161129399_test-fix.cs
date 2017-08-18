namespace SF.Core.net46.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testfix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Post", "UserId", "dbo.User");
            DropIndex("dbo.User", new[] { "FirstName" });
            DropIndex("dbo.User", "full");
            DropIndex("dbo.Post", new[] { "UserId" });
            CreateIndex("dbo.User", new[] { "FirstName", "LastName" }, name: "full");
            CreateIndex("dbo.Post", "UserId");
            AddForeignKey("dbo.Post", "UserId", "dbo.User", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Post", "UserId", "dbo.User");
            DropIndex("dbo.Post", new[] { "UserId" });
            DropIndex("dbo.User", "full");
            CreateIndex("dbo.Post", "UserId");
            CreateIndex("dbo.User", new[] { "FirstName", "LastName" }, name: "full");
            CreateIndex("dbo.User", "FirstName");
            AddForeignKey("dbo.Post", "UserId", "dbo.User", "Id", cascadeDelete: true);
        }
    }
}
