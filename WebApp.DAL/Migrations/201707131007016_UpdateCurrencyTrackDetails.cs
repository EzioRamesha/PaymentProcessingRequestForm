namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCurrencyTrackDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Currencies", "PreviousCurrencyId", c => c.Guid());
            CreateIndex("dbo.Currencies", "PreviousCurrencyId");
            AddForeignKey("dbo.Currencies", "PreviousCurrencyId", "dbo.Currencies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Currencies", "PreviousCurrencyId", "dbo.Currencies");
            DropIndex("dbo.Currencies", new[] { "PreviousCurrencyId" });
            DropColumn("dbo.Currencies", "PreviousCurrencyId");
        }
    }
}
