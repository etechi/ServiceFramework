namespace SF.Core.net46.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 100),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.FirstName)
                .Index(t => new { t.FirstName, t.LastName }, name: "full");
            
            CreateTable(
                "dbo.Post",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 100),
                        UserId = c.String(nullable: false, maxLength: 100),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Post", "UserId", "dbo.User");
            DropIndex("dbo.Post", new[] { "UserId" });
            DropIndex("dbo.User", "full");
            DropIndex("dbo.User", new[] { "FirstName" });
            DropTable("dbo.Post");
            DropTable("dbo.User");
        }
    }
}
