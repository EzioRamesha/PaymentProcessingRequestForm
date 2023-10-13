namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPPRFNoField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequestForms", "PPRFNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentRequestForms", "PPRFNo");
        }
    }
}
