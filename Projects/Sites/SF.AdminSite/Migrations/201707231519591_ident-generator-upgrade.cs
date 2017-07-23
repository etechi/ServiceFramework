namespace SF.AdminSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class identgeneratorupgrade : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.SysIdentSeed");
            AddColumn("dbo.SysIdentSeed", "ScopeId", c => c.Long(nullable: false));
            AddColumn("dbo.SysIdentSeed", "Section", c => c.Int(nullable: false));
            AlterColumn("dbo.SysIdentSeed", "Type", c => c.String(nullable: false, maxLength: 100));
            AddPrimaryKey("dbo.SysIdentSeed", new[] { "ScopeId", "Type" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.SysIdentSeed");
            AlterColumn("dbo.SysIdentSeed", "Type", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.SysIdentSeed", "Section");
            DropColumn("dbo.SysIdentSeed", "ScopeId");
            AddPrimaryKey("dbo.SysIdentSeed", "Type");
        }
    }
}
