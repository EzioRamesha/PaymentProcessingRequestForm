namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHotelNameAndCountry_Payee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payees", "CountryId", c => c.Guid());
            AddColumn("dbo.Payees", "HotelName", c => c.String());
            CreateIndex("dbo.Payees", "CountryId");
            AddForeignKey("dbo.Payees", "CountryId", "dbo.Countries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payees", "CountryId", "dbo.Countries");
            DropIndex("dbo.Payees", new[] { "CountryId" });
            DropColumn("dbo.Payees", "HotelName");
            DropColumn("dbo.Payees", "CountryId");
        }
    }
}
