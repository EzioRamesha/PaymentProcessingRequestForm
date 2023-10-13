namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addClosedById : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequestForms", "ClosedById", c => c.Guid());
            CreateIndex("dbo.PaymentRequestForms", "ClosedById");
            AddForeignKey("dbo.PaymentRequestForms", "ClosedById", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentRequestForms", "ClosedById", "dbo.Users");
            DropIndex("dbo.PaymentRequestForms", new[] { "ClosedById" });
            DropColumn("dbo.PaymentRequestForms", "ClosedById");
        }
    }
}
