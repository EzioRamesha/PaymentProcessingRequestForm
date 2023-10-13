namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDocumentType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequestForms", "DocumentType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentRequestForms", "DocumentType");
        }
    }
}
