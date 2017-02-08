namespace SF.AdminSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SysServiceInstance",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 100),
                        DeclarationId = c.String(nullable: false, maxLength: 50),
                        ImplementId = c.String(nullable: false, maxLength: 50),
                        IsDefaultService = c.Boolean(nullable: false),
                        LogicState = c.Byte(nullable: false),
                        CreateArguments = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.DeclarationId, t.IsDefaultService }, name: "service")
                .Index(t => t.ImplementId);
            
            CreateTable(
                "dbo.SysIdentSeed",
                c => new
                    {
                        Type = c.String(nullable: false, maxLength: 128),
                        NextValue = c.Long(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Type);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.SysServiceInstance", new[] { "ImplementId" });
            DropIndex("dbo.SysServiceInstance", "service");
            DropTable("dbo.SysIdentSeed");
            DropTable("dbo.SysServiceInstance");
        }
    }
}
