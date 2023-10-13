namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CountryOptionalInNewRequest : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PaymentRequestForms", "CountryId", "dbo.Countries");
            DropIndex("dbo.PaymentRequestForms", new[] { "CountryId" });
            AlterColumn("dbo.PaymentRequestForms", "CountryId", c => c.Guid());
            CreateIndex("dbo.PaymentRequestForms", "CountryId");
            AddForeignKey("dbo.PaymentRequestForms", "CountryId", "dbo.Countries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentRequestForms", "CountryId", "dbo.Countries");
            DropIndex("dbo.PaymentRequestForms", new[] { "CountryId" });
            AlterColumn("dbo.PaymentRequestForms", "CountryId", c => c.Guid(nullable: false));
            CreateIndex("dbo.PaymentRequestForms", "CountryId");
            AddForeignKey("dbo.PaymentRequestForms", "CountryId", "dbo.Countries", "Id", cascadeDelete: true);
        }
    }
}
