namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomExchangeRates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequestForms", "USDExRate", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AddColumn("dbo.PaymentRequestForms", "EuroExRate", c => c.Decimal(nullable: false, precision: 18, scale: 6));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentRequestForms", "EuroExRate");
            DropColumn("dbo.PaymentRequestForms", "USDExRate");
        }
    }
}
