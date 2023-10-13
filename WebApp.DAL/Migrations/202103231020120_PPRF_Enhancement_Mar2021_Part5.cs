namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PPRF_Enhancement_Mar2021_Part5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PaymentRequestForms", "PayeeBankAccountDetailId", "dbo.PayeeBankAccountDetails");
            DropIndex("dbo.PaymentRequestForms", new[] { "PayeeBankAccountDetailId" });
            AlterColumn("dbo.PaymentRequestForms", "PayeeBankAccountDetailId", c => c.Guid());
            CreateIndex("dbo.PaymentRequestForms", "PayeeBankAccountDetailId");
            AddForeignKey("dbo.PaymentRequestForms", "PayeeBankAccountDetailId", "dbo.PayeeBankAccountDetails", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentRequestForms", "PayeeBankAccountDetailId", "dbo.PayeeBankAccountDetails");
            DropIndex("dbo.PaymentRequestForms", new[] { "PayeeBankAccountDetailId" });
            AlterColumn("dbo.PaymentRequestForms", "PayeeBankAccountDetailId", c => c.Guid(nullable: false));
            CreateIndex("dbo.PaymentRequestForms", "PayeeBankAccountDetailId");
            AddForeignKey("dbo.PaymentRequestForms", "PayeeBankAccountDetailId", "dbo.PayeeBankAccountDetails", "Id", cascadeDelete: true);
        }
    }
}
