namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeDocumentPathToSavedName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequestForms", "DocumentSavedName", c => c.String());
            DropColumn("dbo.PaymentRequestForms", "DocumentPath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PaymentRequestForms", "DocumentPath", c => c.String());
            DropColumn("dbo.PaymentRequestForms", "DocumentSavedName");
        }
    }
}
