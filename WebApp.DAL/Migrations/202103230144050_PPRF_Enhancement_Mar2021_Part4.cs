namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PPRF_Enhancement_Mar2021_Part4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequestForms", "RestrictedPayeeOnly", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentRequestForms", "RestrictedPayeeOnly");
        }
    }
}
