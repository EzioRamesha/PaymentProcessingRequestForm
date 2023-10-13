namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClosingForPPRF : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequestForms", "IsClosed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentRequestForms", "IsClosed");
        }
    }
}
