namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusAndIsCancelledFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequestForms", "Status", c => c.String());
            AddColumn("dbo.PaymentRequestForms", "IsCancelled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentRequestForms", "IsCancelled");
            DropColumn("dbo.PaymentRequestForms", "Status");
        }
    }
}
