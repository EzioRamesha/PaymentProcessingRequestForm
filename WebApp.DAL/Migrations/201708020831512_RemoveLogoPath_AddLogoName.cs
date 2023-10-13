namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLogoPath_AddLogoName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayingEntities", "LogoName", c => c.String());
            DropColumn("dbo.PayingEntities", "LogoPath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PayingEntities", "LogoPath", c => c.String());
            DropColumn("dbo.PayingEntities", "LogoName");
        }
    }
}
