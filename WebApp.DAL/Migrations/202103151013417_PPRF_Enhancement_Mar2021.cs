namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PPRF_Enhancement_Mar2021 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequestForms", "BudgetApprovedAmt", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AddColumn("dbo.PaymentRequestForms", "BudgetApprovedAmtUSD", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AddColumn("dbo.PaymentRequestForms", "BudgetApprovedAmtEuro", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AddColumn("dbo.PaymentRequestForms", "BudgetSpentAmt", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AddColumn("dbo.PaymentRequestForms", "BudgetSpentAmtUSD", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AddColumn("dbo.PaymentRequestForms", "BudgetSpentAmtEuro", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AddColumn("dbo.PaymentRequestForms", "BudgetPPRFNo_Id", c => c.Guid());
            CreateIndex("dbo.PaymentRequestForms", "BudgetPPRFNo_Id");
            AddForeignKey("dbo.PaymentRequestForms", "BudgetPPRFNo_Id", "dbo.PaymentRequestForms", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentRequestForms", "BudgetPPRFNo_Id", "dbo.PaymentRequestForms");
            DropIndex("dbo.PaymentRequestForms", new[] { "BudgetPPRFNo_Id" });
            DropColumn("dbo.PaymentRequestForms", "BudgetPPRFNo_Id");
            DropColumn("dbo.PaymentRequestForms", "BudgetSpentAmtEuro");
            DropColumn("dbo.PaymentRequestForms", "BudgetSpentAmtUSD");
            DropColumn("dbo.PaymentRequestForms", "BudgetSpentAmt");
            DropColumn("dbo.PaymentRequestForms", "BudgetApprovedAmtEuro");
            DropColumn("dbo.PaymentRequestForms", "BudgetApprovedAmtUSD");
            DropColumn("dbo.PaymentRequestForms", "BudgetApprovedAmt");
        }
    }
}
