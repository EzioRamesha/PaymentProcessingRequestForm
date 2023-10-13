namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PPRF_Enhancement_Dec2020_P1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentRequestDocuments",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        DocumentName = c.String(),
                        DocumentSavedName = c.String(),
                        PaymentRequestFormId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PaymentRequestForms", t => t.PaymentRequestFormId, cascadeDelete: true)
                .Index(t => t.PaymentRequestFormId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentRequestDocuments", "PaymentRequestFormId", "dbo.PaymentRequestForms");
            DropIndex("dbo.PaymentRequestDocuments", new[] { "PaymentRequestFormId" });
            DropTable("dbo.PaymentRequestDocuments");
        }
    }
}
